using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MandelbrotApp
{
    public partial class MainWindow : Window
    {
        // Начальные параметры для множества Мандельброта
        // Используются для сброса к начальному виду (клавиша Space)
        decimal initialCenterX = -0.5m; // Начальная X-координата центра
        decimal initialCenterY = 0m;    // Начальная Y-координата центра
        decimal initialScale = 3m;      // Начальный масштаб (ширина области в комплексной плоскости)

        // Текущие параметры отображения, изменяются при зуме и перемещении
        decimal centerX;                // Текущая X-координата центра изображения
        decimal centerY;                // Текущая Y-координата центра изображения
        decimal scale;                  // Текущий масштаб (ширина области в комплексной плоскости)
        int maxIterations = 100;        // Максимальное число итераций для алгоритма Мандельброта

        WriteableBitmap bitmap;         // Битовая карта для отрисовки изображения
        int width, height;              // Размеры изображения в пикселях

        // Объект для управления отменой задач рендеринга
        CancellationTokenSource cts = new CancellationTokenSource();

        // Таймер для непрерывного зума при зажатой кнопке мыши
        DispatcherTimer zoomTimer;      // Управляет периодическим вызовом зума
        Point lastClickPoint;           // Последняя позиция клика мыши
        bool isZooming = false;         // Флаг, указывающий, выполняется ли зум

        // Параметры для контролируемого пула потоков
        readonly int threadCount;       // Оптимальное число потоков для Parallel.For
        readonly ParallelOptions parallelOptions; // Настройки для Parallel.For

        /// <summary>
        /// Конструктор окна. Инициализирует параметры, пул потоков и таймер для зума.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Установка начальных значений текущих параметров
            centerX = initialCenterX;
            centerY = initialCenterY;
            scale = initialScale;

            // Настройка пула потоков
            threadCount = Math.Max(1, Environment.ProcessorCount - 1); // Оставляем одно ядро для UI
            ThreadPool.SetMinThreads(threadCount, threadCount); // Минимальное число потоков
            ThreadPool.SetMaxThreads(threadCount * 2, threadCount * 2); // Максимальное число потоков

            // Настройка Parallel.For для ограничения числа потоков
            parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = threadCount // Ограничиваем параллелизм
            };

            // Подписка на событие загрузки окна
            Loaded += MainWindow_Loaded;

            // Настройка таймера для плавного зума
            zoomTimer = new DispatcherTimer();
            zoomTimer.Interval = TimeSpan.FromMilliseconds(300); // Зум каждые 300 мс
            zoomTimer.Tick += ZoomTimer_Tick;
        }

        /// <summary>
        /// Обработчик события загрузки окна. Инициализирует битмап и запускает рендеринг.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Определяем размеры изображения на основе размеров элемента UI
            width = (int)mandelbrotImage.ActualWidth;
            height = (int)mandelbrotImage.ActualHeight;

            // Если размеры не определены, используем значения по умолчанию
            if (width == 0 || height == 0)
            {
                width = 1520;
                height = 900;
            }

            // Создаём WriteableBitmap для отображения множества Мандельброта
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            mandelbrotImage.Source = bitmap;

            // Запускаем первую генерацию изображения
            RenderMandelbrot();
        }

        /// <summary>
        /// Запускает генерацию множества Мандельброта в фоновом потоке с прогрессивным улучшением качества.
        /// Каждый вызов отменяет предыдущую задачу рендеринга.
        /// </summary>
        private void RenderMandelbrot()
        {
            // Отменяем текущую задачу рендеринга
            cts.Cancel();
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Запускаем вычисления в фоновом потоке, чтобы не блокировать UI
            Task.Run(() =>
            {
                // Массив уровней качества: от грубого (16) до полного (1)
                int[] qualityLevels = { 16, 8, 4, 2, 1 };
                foreach (int quality in qualityLevels)
                {
                    int stride = width * 4;                     // Stride — это длина строки в байтах для одномерного
                                                                // массива пикселей
                                                                // (ширина * 4 байта на пиксель: B, G, R, A)

                    byte[] pixels = new byte[height * stride];  // Одномерный массив для хранения пикселей изображения;
                                                                // используется вместо двумерного для совместимости с
                                                                // WritePixels и оптимизации доступа к памяти
                    if (quality == 1)
                    {
                        // Финальный рендеринг: разбиваем изображение на блоки по 100 строк
                        int blockSize = 100;
                        for (int blockStart = 0; blockStart < height; blockStart += blockSize)
                        {
                            int blockEnd = Math.Min(blockStart + blockSize, height);

                            // Параллельно вычисляем строки в блоке с ограниченным числом потоков
                            Parallel.For(blockStart, blockEnd, parallelOptions, y =>
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                for (int x = 0; x < width; x++)
                                {
                                    // a и b — действительная и мнимая части комплексного числа c,
                                    // соответствующего координатам пикселя (x, y) на комплексной плоскости
                                    decimal a = centerX + (x - width / 2.0m) * (scale / width);
                                    decimal b = centerY + (y - height / 2.0m) * (scale / width);
                                    int iterations = Mandelbrot(a, b, maxIterations);
                                    int color = GetColor(iterations, maxIterations);

                                    // Записываем цвет в массив пикселей
                                    int index = y * stride + x * 4; // Индекс в одномерном массиве:
                                                                    // y * stride переходит к строке,
                                                                    // x * 4 — к пикселю в строке
                                    pixels[index] = (byte)(color & 0xFF);         // Синий
                                    pixels[index + 1] = (byte)((color >> 8) & 0xFF);  // Зелёный
                                    pixels[index + 2] = (byte)((color >> 16) & 0xFF); // Красный
                                    pixels[index + 3] = 255;                          // Альфа
                                }
                            });

                            // Обновляем блок изображения в основном потоке
                            Dispatcher.Invoke(() =>
                            {
                                Int32Rect rect = new Int32Rect(0, blockStart, width, blockEnd - blockStart);
                                bitmap.WritePixels(rect, pixels, stride, blockStart * stride); // stride указывает длину строки для правильного чтения из одномерного массива
                            });

                            if (token.IsCancellationRequested)
                                return;
                        }
                    }
                    else
                    {
                        // Грубыe этапы: вычисляем каждый quality-й пиксель
                        Parallel.For(0, height, parallelOptions, y =>
                        {
                            if (token.IsCancellationRequested)
                                return;

                            for (int x = 0; x < width; x++)
                            {
                                if (x % quality != 0 || y % quality != 0)
                                    continue;

                                // a и b представляют точку c на комплексной плоскости, для которой проверяется принадлежность множеству Мандельброта
                                decimal a = centerX + (x - width / 2.0m) * (scale / width);
                                decimal b = centerY + (y - height / 2.0m) * (scale / width);
                                int iterations = Mandelbrot(a, b, maxIterations);
                                int color = GetColor(iterations, maxIterations);

                                int index = y * stride + x * 4;
                                pixels[index] = (byte)(color & 0xFF);
                                pixels[index + 1] = (byte)((color >> 8) & 0xFF);
                                pixels[index + 2] = (byte)((color >> 16) & 0xFF);
                                pixels[index + 3] = 255;

                                // Заполняем соседние пиксели для грубого изображения
                                for (int dy = 0; dy < quality && (y + dy) < height; dy++)
                                {
                                    for (int dx = 0; dx < quality && (x + dx) < width; dx++)
                                    {
                                        int idx = (y + dy) * stride + (x + dx) * 4; // Индексация с учётом stride для доступа к соседним пикселям
                                        pixels[idx] = (byte)(color & 0xFF);
                                        pixels[idx + 1] = (byte)((color >> 8) & 0xFF);
                                        pixels[idx + 2] = (byte)((color >> 16) & 0xFF);
                                        pixels[idx + 3] = 255;
                                    }
                                }
                            }
                        });

                        // Обновляем всё изображение
                        Dispatcher.Invoke(() =>
                        {
                            Int32Rect rect = new Int32Rect(0, 0, width, height);
                            bitmap.WritePixels(rect, pixels, stride, 0);
                        });
                    }

                    if (token.IsCancellationRequested)
                        break;
                }
            }, token);
        }

        /// <summary>
        /// Вычисляет количество итераций для точки (a, b) в множестве Мандельброта.
        /// </summary>
        /// <param name="a">Действительная часть комплексного числа</param>
        /// <param name="b">Мнимая часть комплексного числа</param>
        /// <param name="maxIter">Максимальное число итераций</param>
        /// <returns>Число итераций до выхода за пределы или maxIter</returns>
        private int Mandelbrot(decimal a, decimal b, int maxIter)
        {
            decimal ca = a;
            decimal cb = b;
            int iter = 0;

            for (; iter < maxIter; iter++)
            {
                decimal aa = a * a - b * b;
                decimal bb = 2 * a * b;
                a = ca + aa;
                b = cb + bb;
                if (a * a + b * b > 16)
                    break;
            }
            return iter;
        }

        /// <summary>
        /// Преобразует число итераций в цвет для визуализации.
        /// </summary>
        /// <param name="iter">Число итераций</param>
        /// <param name="maxIter">Максимальное число итераций</param>
        /// <returns>Цвет в формате ARGB</returns>
        private int GetColor(int iter, int maxIter)
        {
            if (iter == maxIter)
                return 0x000000;

            int r = (iter * 9) % 256;
            int g = (iter * 7) % 256;
            int b = (iter * 5) % 256;
            return (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Обработчик нажатия мыши: запускает зум в точке клика.
        /// </summary>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isZooming = true;
            lastClickPoint = e.GetPosition(mandelbrotImage);
            ZoomToRect(lastClickPoint);
            zoomTimer.Start();
        }

        /// <summary>
        /// Обработчик движения мыши: обновляет позицию зума при зажатой кнопке.
        /// </summary>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isZooming)
            {
                lastClickPoint = e.GetPosition(mandelbrotImage);
            }
        }

        /// <summary>
        /// Обработчик отпускания мыши: останавливает зум.
        /// </summary>
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isZooming = false;
            zoomTimer.Stop();
        }

        /// <summary>
        /// Обработчик тика таймера: выполняет зум в текущей позиции курсора.
        /// </summary>
        private void ZoomTimer_Tick(object sender, EventArgs e)
        {
            if (isZooming)
            {
                ZoomToRect(lastClickPoint);
            }
        }

        /// <summary>
        /// Выполняет зум в прямоугольной области вокруг точки клика.
        /// </summary>
        private void ZoomToRect(Point clickPoint)
        {
            decimal rectWidth = 0.9m * width;
            decimal rectHeight = 0.9m * height;

            decimal x0 = (decimal)clickPoint.X - rectWidth / 2;
            decimal y0 = (decimal)clickPoint.Y - rectHeight / 2;

            if (x0 < 0) x0 = 0;
            if (y0 < 0) y0 = 0;
            if (x0 + rectWidth > width) x0 = width - rectWidth;
            if (y0 + rectHeight > height) y0 = height - rectHeight;

            decimal newCenterX = centerX + ((x0 + rectWidth / 2) - (width / 2.0m)) * (scale / width);
            decimal newCenterY = centerY + ((y0 + rectHeight / 2) - (height / 2.0m)) * (scale / width);
            decimal newScale = (rectWidth / width) * scale;

            centerX = newCenterX;
            centerY = newCenterY;
            scale = newScale;

            maxIterations = (int)(maxIterations * 1.01m);
            RenderMandelbrot();
        }

        /// <summary>
        /// Обработчик нажатия клавиш: управление приложением.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
            else if (e.Key == Key.Space)
            {
                centerX = initialCenterX;
                centerY = initialCenterY;
                scale = initialScale;
                maxIterations = 100;
                RenderMandelbrot();
            }
            else if (e.Key == Key.Left)
            {
                centerX -= 0.1m * scale;
                RenderMandelbrot();
            }
            else if (e.Key == Key.Right)
            {
                centerX += 0.1m * scale;
                RenderMandelbrot();
            }
            else if (e.Key == Key.Up)
            {
                centerY -= 0.1m * scale;
                RenderMandelbrot();
            }
            else if (e.Key == Key.Down)
            {
                centerY += 0.1m * scale;
                RenderMandelbrot();
            }
        }
    }
}