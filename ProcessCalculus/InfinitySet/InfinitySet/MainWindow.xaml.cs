using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;

namespace MandelbrotApp
{
    public partial class MainWindow : Window
    {
        // Начальные параметры для множества Мандельброта
        // Используются для сброса к начальному виду (клавиша Space)
        private readonly decimal initialCenterX = -0.5m; // Начальная X-координата центра
        private readonly decimal initialCenterY = 0m;    // Начальная Y-координата центра
        private readonly decimal initialScale = 3m;      // Начальный масштаб (ширина области в комплексной плоскости)

        // Текущие параметры отображения, изменяются при зуме и перемещении
        private decimal centerX;                // Текущая X-координата центра изображения
        private decimal centerY;                // Текущая Y-координата центра изображения
        private decimal scale;                  // Текущий масштаб (ширина области в комплексной плоскости)
        private int maxIterations = 100;        // Максимальное число итераций для алгоритма Мандельброта

        private WriteableBitmap bitmap;         // Битовая карта для отрисовки изображения
        private int width, height;              // Размеры изображения в пикселях

        // Объект для управления отменой задач рендеринга
        private CancellationTokenSource cts = new CancellationTokenSource();

        // Таймер для непрерывного зума при зажатой кнопке мыши
        private readonly DispatcherTimer zoomTimer; // Управляет периодическим вызовом зума
        private Point lastClickPoint;           // Последняя позиция клика мыши
        private bool isZooming = false;         // Флаг, указывающий, выполняется ли зум

        // Параметры для контролируемого пула потоков
        private readonly int threadCount;       // Оптимальное число потоков для Parallel.For
        private readonly ParallelOptions parallelOptions; // Настройки для Parallel.For

        // Стрим для записи времени в файл
        private StreamWriter timeLogger;

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

            // Открытие файла для логирования времени рендеринга
            timeLogger = new StreamWriter("render_times.txt", append: true);
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
        /// Алгоритм:
        /// 1. Отменяет текущую задачу рендеринга через CancellationTokenSource.
        /// 2. Использует прогрессивный рендеринг с несколькими уровнями качества (от грубого к детализированному).
        /// 3. Для каждого уровня качества вычисляет пиксели параллельно с помощью Parallel.For.
        /// 4. Для финального рендеринга разбивает изображение на блоки для оптимизации обновления UI.
        /// 5. Обновляет изображение в основном потоке через Dispatcher.Invoke.
        /// </summary>
        private void RenderMandelbrot()
        {
            // Используем Stopwatch для точного замера времени в миллисекундах
            var totalStopwatch = Stopwatch.StartNew();

            // Записываем время начала рендеринга
            timeLogger.WriteLine(new string('-', 50));
            timeLogger.WriteLine($"Render started at {DateTime.Now:HH:mm:ss.fff}");
            timeLogger.WriteLine($"Total rendering process");
            timeLogger.WriteLine(new string('-', 50));

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
                    // Создаем Stopwatch для текущего уровня качества
                    var stageStopwatch = Stopwatch.StartNew();

                    // Записываем начало этапа в отдельном блоке
                    timeLogger.WriteLine(new string('=', 50));
                    timeLogger.WriteLine($"Quality level: {quality}");
                    timeLogger.WriteLine($"Started at: {DateTime.Now:HH:mm:ss.fff}");
                    timeLogger.WriteLine(new string('=', 50));

                    int stride = width * 4; // Длина строки в байтах (ширина * 4 байта на пиксель: B, G, R, A)
                    byte[] pixels = new byte[height * stride]; // Массив для хранения пикселей изображения

                    if (quality == 1)
                    {
                        // Финальный рендеринг: разбиваем изображение на блоки по 100 строк
                        int blockSize = 100;
                        for (int blockStart = 0; blockStart < height; blockStart += blockSize)
                        {
                            int blockEnd = Math.Min(blockStart + blockSize, height);

                            // Параллельно вычисляем строки в блоке
                            Parallel.For(blockStart, blockEnd, parallelOptions, y =>
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                for (int x = 0; x < width; x++)
                                {
                                    // Преобразуем координаты пикселя в точку на комплексной плоскости
                                    decimal a = centerX + (x - width / 2.0m) * (scale / width);
                                    decimal b = centerY + (y - height / 2.0m) * (scale / width);
                                    int iterations = MandelbrotCalculator.Mandelbrot(a, b, maxIterations);
                                    int color = MandelbrotCalculator.GetColor(iterations, maxIterations);

                                    // Записываем цвет в массив пикселей
                                    int index = y * stride + x * 4;
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
                                bitmap.WritePixels(rect, pixels, stride, blockStart * stride);
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

                                // Преобразуем координаты пикселя в точку на комплексной плоскости
                                decimal a = centerX + (x - width / 2.0m) * (scale / width);
                                decimal b = centerY + (y - height / 2.0m) * (scale / width);
                                int iterations = MandelbrotCalculator.Mandelbrot(a, b, maxIterations);
                                int color = MandelbrotCalculator.GetColor(iterations, maxIterations);

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
                                        int idx = (y + dy) * stride + (x + dx) * 4;
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

                    // Останавливаем таймер этапа и записываем время окончания
                    stageStopwatch.Stop();
                    timeLogger.WriteLine($"Ended at: {DateTime.Now:HH:mm:ss.fff}");
                    timeLogger.WriteLine($"Duration: {stageStopwatch.ElapsedMilliseconds} ms");
                    timeLogger.WriteLine(new string('=', 50));
                    timeLogger.Flush();

                    if (token.IsCancellationRequested)
                        break;
                }

                // Останавливаем общий таймер и записываем общее время
                totalStopwatch.Stop();
                timeLogger.WriteLine(new string('-', 50));
                timeLogger.WriteLine($"Render ended at {DateTime.Now:HH:mm:ss.fff}");
                timeLogger.WriteLine($"Total render time: {totalStopwatch.ElapsedMilliseconds} ms");
                timeLogger.WriteLine(new string('-', 50));
                timeLogger.Flush();
            }, token);
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
        /// Алгоритм:
        /// 1. Определяет прямоугольник размером 90% от текущего изображения, центрированный в точке клика.
        /// 2. Корректирует координаты прямоугольника, чтобы он не выходил за границы изображения.
        /// 3. Вычисляет новый центр изображения и масштаб на основе прямоугольника.
        /// 4. Увеличивает количество итераций для повышения детализации.
        /// 5. Запускает рендеринг с новыми параметрами.
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
        /// Обработчик нажатия клавиши пробела: сбрасывает изображение в исходное состояние.
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
