using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GeneticAlgorithmTSP
{
    public partial class MainWindow : Window
    {
        // Список городов (координаты)
        private List<Point> cities;
        // Текущая популяция
        private List<Individual> population;
        // Лучший маршрут
        private Individual bestIndividual;
        // Флаг для остановки алгоритма
        private volatile bool isRunning;
        // Диспетчер для обновления UI
        private Dispatcher uiDispatcher;
        // Текущий номер поколения
        private int generation;
        // Размер сетки
        private const double GridSize = 50;
        // Радиус для определения клика по точке (для удаления)
        private const double ClickRadius = 8;

        public MainWindow()
        {
            InitializeComponent();
            uiDispatcher = Dispatcher.CurrentDispatcher;
            cities = new List<Point>();
            DrawGrid();
            UpdateCitiesCount();
        }

        // Обработчик клика по канвасу для добавления или удаления города
        private void RouteCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isRunning)
            {
                Point clickPoint = e.GetPosition(RouteCanvas);

                // Проверка, находится ли клик рядом с существующей точкой (для удаления)
                Point? pointToRemove = null;
                foreach (var city in cities)
                {
                    double distance = CalculateDistance(clickPoint, city);
                    if (distance <= ClickRadius)
                    {
                        pointToRemove = city;
                        break;
                    }
                }

                if (pointToRemove.HasValue)
                {
                    // Удаление точки
                    cities.Remove(pointToRemove.Value);
                }
                else
                {
                    // Проверка, нет ли уже точки в этой позиции
                    bool pointExists = cities.Any(city => CalculateDistance(city, clickPoint) < 1.0);
                    if (!pointExists && clickPoint.X >= 0 && clickPoint.X <= RouteCanvas.Width && clickPoint.Y >= 0 && clickPoint.Y <= RouteCanvas.Height)
                    {
                        // Добавление новой точки
                        cities.Add(clickPoint);
                    }
                }

                DrawCities();
                UpdateCitiesCount();
            }
        }

        // Обработчик кнопки "Сгенерировать точки"
        private void GenerateCitiesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                cities.Clear();
                Random rand = new Random();
                int cityCount = rand.Next(10, 21); // Случайное количество городов от 10 до 20
                for (int i = 0; i < cityCount; i++)
                {
                    double x = rand.NextDouble() * RouteCanvas.Width;
                    double y = rand.NextDouble() * RouteCanvas.Height;
                    Point newPoint = new Point(x, y);

                    // Проверка, чтобы точка не совпадала с уже существующими
                    bool pointExists = cities.Any(city => CalculateDistance(city, newPoint) < 1.0);
                    if (!pointExists)
                    {
                        cities.Add(newPoint);
                    }
                    else
                    {
                        i--; // Повторить попытку, если точка совпала
                    }
                }
                DrawCities();
                UpdateCitiesCount();
            }
        }

        // Обработчик кнопки "Очистить точки"
        private void ClearCitiesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                cities.Clear();
                RouteCanvas.Children.Clear();
                DrawGrid();
                UpdateCitiesCount();
            }
        }

        // Обновление количества точек в UI
        private void UpdateCitiesCount()
        {
            uiDispatcher.Invoke(() =>
            {
                CitiesCountTextBlock.Text = cities.Count.ToString();
            });
        }

        // Отрисовка координатной сетки
        private void DrawGrid()
        {
            RouteCanvas.Children.Clear();

            // Отрисовка линий сетки
            for (double x = 0; x <= RouteCanvas.Width; x += GridSize)
            {
                var line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = RouteCanvas.Height,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                RouteCanvas.Children.Add(line);

                // Подписи по оси X
                var text = new TextBlock
                {
                    Text = x.ToString(),
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(text, x - 10);
                Canvas.SetTop(text, RouteCanvas.Height + 5);
                RouteCanvas.Children.Add(text);
            }

            for (double y = 0; y <= RouteCanvas.Height; y += GridSize)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = RouteCanvas.Width,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                RouteCanvas.Children.Add(line);

                // Подписи по оси Y
                var text = new TextBlock
                {
                    Text = y.ToString(),
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(text, -30);
                Canvas.SetTop(text, y - 10);
                RouteCanvas.Children.Add(text);
            }

            // Оси X и Y
            var xAxis = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = RouteCanvas.Width,
                Y2 = 0,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            var yAxis = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = RouteCanvas.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            RouteCanvas.Children.Add(xAxis);
            RouteCanvas.Children.Add(yAxis);
        }

        // Отрисовка городов
        private void DrawCities()
        {
            RouteCanvas.Children.Clear();
            DrawGrid();
            foreach (var city in cities)
            {
                var ellipse = new Ellipse { Width = 8, Height = 8, Fill = Brushes.Red };
                Canvas.SetLeft(ellipse, city.X - 4);
                Canvas.SetTop(ellipse, city.Y - 4);
                RouteCanvas.Children.Add(ellipse);
            }
        }

        // Обработчик нажатия кнопки "Запустить"
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (cities.Count < 4)
            {
                MessageBox.Show("Добавьте хотя бы 4 точки!");
                return;
            }
            if (!int.TryParse(ThreadsCountTextBox.Text, out int threadsCount) || threadsCount < 1)
            {
                MessageBox.Show("Введите корректное число потоков!");
                return;
            }
            if (!int.TryParse(PopulationSizeTextBox.Text, out int populationSize) || populationSize < 10)
            {
                MessageBox.Show("Введите корректный размер популяции (не менее 10)!");
                return;
            }

            isRunning = true;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            GenerateCitiesButton.IsEnabled = false;
            ClearCitiesButton.IsEnabled = false;

            // Запуск генетического алгоритма в отдельном потоке
            Task.Run(() => RunGeneticAlgorithm(threadsCount, populationSize));
        }

        // Обработчик нажатия кнопки "Остановить"
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = true;
            GenerateCitiesButton.IsEnabled = true;
            ClearCitiesButton.IsEnabled = true;
        }

        // Основной метод генетического алгоритма с явным использованием ThreadPool
        private void RunGeneticAlgorithm(int threadsCount, int populationSize)
        {
            // Инициализация начальной популяции
            population = InitializePopulation(populationSize);
            bestIndividual = population.OrderBy(ind => ind.Fitness).First();
            generation = 0;

            // Явная установка максимального числа потоков в ThreadPool
            ThreadPool.SetMaxThreads(threadsCount, threadsCount);

            // Основной цикл алгоритма
            while (isRunning)
            {
                generation++;
                var newPopulation = new List<Individual> { bestIndividual }; // Элитизм: сохраняем лучший маршрут

                // Объект для синхронизации добавления в новую популяцию
                var populationLock = new object();
                // Счётчик завершённых задач
                var completedTasks = 0;
                var tasksLock = new object();
                var waitHandle = new ManualResetEvent(false);

                // Разделение работы на подзадачи
                int individualsPerThread = (populationSize - 1) / threadsCount;
                for (int i = 0; i < threadsCount; i++)
                {
                    int startIndex = i * individualsPerThread;
                    int count = i == threadsCount - 1 ? populationSize - 1 - startIndex : individualsPerThread;

                    // Использование ThreadPool
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        // Выбор родителя через турнирную селекцию
                        var parent = TournamentSelection();

                        var localPopulation = new List<Individual>();
                        for (int j = 0; j < count; j++)
                        {

                            // Создание новой особи путём мутации
                            var child = new Individual(parent.Route.ToArray(), parent.Fitness);
                            Mutate(child); // Мутация
                            localPopulation.Add(child);
                        }

                        // Синхронизированное добавление в новую популяцию
                        lock (populationLock)
                        {
                            newPopulation.AddRange(localPopulation);
                        }

                        // Увеличение счётчика завершённых задач
                        lock (tasksLock)
                        {
                            completedTasks++;
                            if (completedTasks == threadsCount)
                            {
                                waitHandle.Set(); // Сигнал завершения всех задач
                            }
                        }
                    });
                }

                // Ожидание завершения всех задач в ThreadPool
                waitHandle.WaitOne();

                // Обновление популяции
                population = newPopulation;
                bestIndividual = population.OrderBy(ind => ind.Fitness).First();

                // Обновление UI в главном потоке
                uiDispatcher.Invoke(() =>
                {
                    GenerationTextBlock.Text = generation.ToString();
                    BestDistanceTextBlock.Text = bestIndividual.Fitness.ToString("F2");
                    DrawPopulation();
                });

                // Задержка для визуализации
                Thread.Sleep(100);

                // Сброс waitHandle для следующей итерации
                waitHandle.Reset();
            }
        }

        // Инициализация начальной популяции
        private List<Individual> InitializePopulation(int size)
        {
            var pop = new List<Individual>();
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                var route = Enumerable.Range(0, cities.Count).OrderBy(x => rand.Next()).ToArray();
                pop.Add(new Individual(route, CalculateFitness(route)));
            }
            return pop;
        }

        // Турнирная селекция
        private Individual TournamentSelection()
        {
            Random rand = new Random();
            var tournamentSize = 5;
            var tournament = population.OrderBy(x => rand.Next()).Take(tournamentSize).ToList();
            return tournament.OrderBy(ind => ind.Fitness).First();
        }

        // Мутация с новой логикой: 25% особей — 2 мутации, 30% особей - 1 мутацию, 10% — случайное количество
        private void Mutate(Individual individual)
        {
            Random rand = new Random();
            double randomValue = rand.NextDouble();

            if (randomValue < 0.15) // 15% особей получают 2 мутации
            {
                // Первая мутация
                int i1 = rand.Next(cities.Count);
                int j1 = rand.Next(cities.Count);
                int temp1 = individual.Route[i1];
                individual.Route[i1] = individual.Route[j1];
                individual.Route[j1] = temp1;

                // Вторая мутация
                int i2 = rand.Next(cities.Count);
                int j2 = rand.Next(cities.Count);
                int temp2 = individual.Route[i2];
                individual.Route[i2] = individual.Route[j2];
                individual.Route[j2] = temp2;
            }
            else if (randomValue < 0.45) // 30% особей получают 1 мутацию
            {

                int i = rand.Next(cities.Count);
                int j = rand.Next(cities.Count);
                int temp = individual.Route[i];
                individual.Route[i] = individual.Route[j];
                individual.Route[j] = temp;
            }
            else if (rand.NextDouble() < 0.55) // 5% получают случайное количество мутаций (от 1 до cities.Count/2)
            {
                int maxMutations = Math.Max(1, cities.Count / 2);
                int mutationCount = rand.Next(1, maxMutations + 1);
                for (int m = 0; m < mutationCount; m++)
                {
                    int i = rand.Next(cities.Count);
                    int j = rand.Next(cities.Count);
                    int temp = individual.Route[i];
                    individual.Route[i] = individual.Route[j];
                    individual.Route[j] = temp;
                }
            }

            // Пересчёт фитнес-функции после мутаций
            individual.Fitness = CalculateFitness(individual.Route);
        }

        // Вычисление пригодности (длина маршрута)
        private double CalculateFitness(int[] route)
        {
            double distance = 0;
            for (int i = 0; i < cities.Count - 1; i++)
            {
                distance += CalculateDistance(cities[route[i]], cities[route[i + 1]]);
            }
            distance += CalculateDistance(cities[route[cities.Count - 1]], cities[route[0]]);
            return distance;
        }

        // Вычисление расстояния между двумя точками
        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // Отрисовка до 20 лучших особей и лучшего маршрута
        private void DrawPopulation()
        {
            RouteCanvas.Children.Clear();
            DrawGrid();
            DrawCities();

            // Выбор до 20 лучших особей (кроме лучшей, которая будет отрисована отдельно)
            var topIndividuals = population.OrderBy(ind => ind.Fitness).Take(21).ToList();
            if (topIndividuals.Contains(bestIndividual))
            {
                topIndividuals.Remove(bestIndividual); // Удаляем лучшую особь, чтобы не дублировать
            }

            // Отрисовка до 20 лучших маршрутов серыми линиями
            foreach (var individual in topIndividuals.Take(20))
            {
                for (int i = 0; i < cities.Count - 1; i++)
                {
                    var line = new Line
                    {
                        X1 = cities[individual.Route[i]].X,
                        Y1 = cities[individual.Route[i]].Y,
                        X2 = cities[individual.Route[i + 1]].X,
                        Y2 = cities[individual.Route[i + 1]].Y,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        Opacity = 0.2 // Прозрачность для читаемости
                    };
                    RouteCanvas.Children.Add(line);
                }
                // Замыкание маршрута
                var closingLine = new Line
                {
                    X1 = cities[individual.Route[cities.Count - 1]].X,
                    Y1 = cities[individual.Route[cities.Count - 1]].Y,
                    X2 = cities[individual.Route[0]].X,
                    Y2 = cities[individual.Route[0]].Y,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    Opacity = 0.2
                };
                RouteCanvas.Children.Add(closingLine);
            }

            // Отрисовка лучшего маршрута синим цветом
            for (int i = 0; i < cities.Count - 1; i++)
            {
                var line = new Line
                {
                    X1 = cities[bestIndividual.Route[i]].X,
                    Y1 = cities[bestIndividual.Route[i]].Y,
                    X2 = cities[bestIndividual.Route[i + 1]].X,
                    Y2 = cities[bestIndividual.Route[i + 1]].Y,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2
                };
                RouteCanvas.Children.Add(line);
            }
            // Замыкание лучшего маршрута
            var closingLineBest = new Line
            {
                X1 = cities[bestIndividual.Route[cities.Count - 1]].X,
                Y1 = cities[bestIndividual.Route[cities.Count - 1]].Y,
                X2 = cities[bestIndividual.Route[0]].X,
                Y2 = cities[bestIndividual.Route[0]].Y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };
            RouteCanvas.Children.Add(closingLineBest);
        }
    }

    // Класс, представляющий особь (индивидуума) в популяции
    public class Individual
    {
        public int[] Route { get; set; }
        public double Fitness { get; set; }

        public Individual(int[] route, double fitness)
        {
            Route = route;
            Fitness = fitness;
        }
    }
}