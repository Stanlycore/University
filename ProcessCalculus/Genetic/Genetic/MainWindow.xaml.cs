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
        // Список городов (точек) на карте
        private List<Point> cities;

        // Текущая популяция маршрутов
        private List<Individual> population;

        // Лучший маршрут в текущей итерации
        private Individual bestIndividual;

        // Флаг для остановки выполнения алгоритма
        private volatile bool isRunning;

        // Диспетчер главного потока, используется для обновления UI
        private Dispatcher uiDispatcher;

        // Номер текущего поколения
        private int generation;

        // Размер ячеек сетки
        private const double GridSize = 50;

        // Радиус клика, чтобы удалить точку
        private const double ClickRadius = 8;

        public MainWindow()
        {
            InitializeComponent();

            // Сохраняем диспетчер UI-потока, чтобы потом обновлять интерфейс из других потоков
            uiDispatcher = Dispatcher.CurrentDispatcher;

            cities = new List<Point>();
            DrawGrid();
            UpdateCitiesCount();
        }

        /// <summary>
        /// Обработчик клика по канвасу: добавляет или удаляет точку.
        /// </summary>
        private void RouteCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isRunning)
            {
                Point clickPoint = e.GetPosition(RouteCanvas);
                Point? pointToRemove = null;

                foreach (var city in cities)
                {
                    if (CalculateDistance(clickPoint, city) <= ClickRadius)
                    {
                        pointToRemove = city;
                        break;
                    }
                }

                if (pointToRemove.HasValue)
                {
                    cities.Remove(pointToRemove.Value);
                }
                else
                {
                    bool pointExists = cities.Any(city => CalculateDistance(city, clickPoint) < 1.0);
                    if (!pointExists && clickPoint.X >= 0 && clickPoint.X <= RouteCanvas.Width &&
                        clickPoint.Y >= 0 && clickPoint.Y <= RouteCanvas.Height)
                    {
                        cities.Add(clickPoint);
                    }
                }

                DrawCities();
                UpdateCitiesCount();
            }
        }

        /// <summary>
        /// Генерирует случайные точки (города).
        /// </summary>
        private void GenerateCitiesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                cities.Clear();
                Random rand = new Random();
                int cityCount = rand.Next(10, 21);

                for (int i = 0; i < cityCount; i++)
                {
                    double x = rand.NextDouble() * RouteCanvas.Width;
                    double y = rand.NextDouble() * RouteCanvas.Height;
                    Point newPoint = new Point(x, y);

                    bool pointExists = cities.Any(city => CalculateDistance(city, newPoint) < 1.0);
                    if (!pointExists)
                    {
                        cities.Add(newPoint);
                    }
                    else
                    {
                        i--;
                    }
                }

                DrawCities();
                UpdateCitiesCount();
            }
        }

        /// <summary>
        /// Очищает все точки.
        /// </summary>
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

        /// <summary>
        /// Обновляет количество точек в UI.
        /// Так как этот метод может вызываться из другого потока, используем Dispatcher.
        /// </summary>
        private void UpdateCitiesCount()
        {
            uiDispatcher.Invoke(() =>
            {
                CitiesCountTextBlock.Text = cities.Count.ToString();
            });
        }

        /// <summary>
        /// Рисует координатную сетку на канвасе.
        /// </summary>
        private void DrawGrid()
        {
            RouteCanvas.Children.Clear();

            for (double x = 0; x <= RouteCanvas.Width; x += GridSize)
            {
                var line = new Line { X1 = x, Y1 = 0, X2 = x, Y2 = RouteCanvas.Height, Stroke = Brushes.LightGray };
                RouteCanvas.Children.Add(line);
                var text = new TextBlock { Text = x.ToString(), Foreground = Brushes.Black };
                Canvas.SetLeft(text, x - 10);
                Canvas.SetTop(text, RouteCanvas.Height + 5);
                RouteCanvas.Children.Add(text);
            }

            for (double y = 0; y <= RouteCanvas.Height; y += GridSize)
            {
                var line = new Line { X1 = 0, Y1 = y, X2 = RouteCanvas.Width, Y2 = y, Stroke = Brushes.LightGray };
                RouteCanvas.Children.Add(line);
                var text = new TextBlock { Text = y.ToString(), Foreground = Brushes.Black };
                Canvas.SetLeft(text, -30);
                Canvas.SetTop(text, y - 10);
                RouteCanvas.Children.Add(text);
            }

            var xAxis = new Line { X1 = 0, Y1 = 0, X2 = RouteCanvas.Width, Y2 = 0, Stroke = Brushes.Black };
            var yAxis = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = RouteCanvas.Height, Stroke = Brushes.Black };
            RouteCanvas.Children.Add(xAxis);
            RouteCanvas.Children.Add(yAxis);
        }

        /// <summary>
        /// Отрисовывает города на канвасе.
        /// </summary>
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

        /// <summary>
        /// Запуск генетического алгоритма в отдельном потоке.
        /// Используется Task.Run(), но можно заменить на Thread.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (cities.Count < 4)
            {
                MessageBox.Show("Добавьте хотя бы 4 точки!");
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

            // 🔁 ВСЁ, ЧТО ПРОИСХОДИТ ДАЛЕЕ, ВЫПОЛНЯЕТСЯ В ФОН. ПОТОКЕ
            Task.Run(() => RunGeneticAlgorithm(populationSize));
        }

        /// <summary>
        /// Останавливает работу алгоритма.
        /// </summary>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
        }

        /// <summary>
        /// Основной цикл генетического алгоритма.
        /// Выполняется в фоновом потоке.
        /// </summary>
        private void RunGeneticAlgorithm(int populationSize)
        {
            // Инициализируем начальную популяцию
            population = InitializePopulation(populationSize);

            // Находим лучшую особь среди начальной популяции
            bestIndividual = population.OrderBy(ind => ind.Fitness).First();

            generation = 0;

            while (isRunning)
            {
                generation++;

                // Элитизм: сохраняем лучшего индивида
                var newPopulation = new List<Individual> { bestIndividual };

                Parallel.For(0, populationSize - 1, index =>
                {
                    // Турнирная селекция: выбираем родителя
                    var parent = TournamentSelection();

                    // Клонируем его маршрут
                    var child = new Individual((int[])parent.Route.Clone(), parent.Fitness);

                    // Применяем мутацию
                    Mutate(child);

                    // Добавляем в новую популяцию с синхронизацией
                    lock (newPopulation)
                    {
                        newPopulation.Add(child);
                    }
                });

                // Обновляем текущую популяцию
                population = newPopulation;

                // Находим лучшего индивида в новой популяции
                bestIndividual = population.OrderBy(ind => ind.Fitness).First();

                // 🔁 ОБНОВЛЕНИЕ UI ИЗ ФОН. ПОТОКА
                // Для этого используем Dispatcher
                uiDispatcher.Invoke(() =>
                {
                    GenerationTextBlock.Text = generation.ToString();
                    BestDistanceTextBlock.Text = bestIndividual.Fitness.ToString("F2");
                    DrawPopulation();
                });

                // Задержка для наглядности
                Thread.Sleep(100);
            }

            // После остановки восстанавливаем кнопки
            uiDispatcher.Invoke(() =>
            {
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                GenerateCitiesButton.IsEnabled = true;
                ClearCitiesButton.IsEnabled = true;
            });
        }

        /// <summary>
        /// Инициализация начальной популяции случайными маршрутами.
        /// </summary>
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

        /// <summary>
        /// Турнирная селекция: выбирает наиболее приспособленного индивида из случайных.
        /// </summary>
        private Individual TournamentSelection()
        {
            Random rand = new Random();
            var tournamentSize = 5;
            var tournament = population.OrderBy(x => rand.Next()).Take(tournamentSize).ToList();
            return tournament.OrderBy(ind => ind.Fitness).First();
        }

        /// <summary>
        /// Мутация маршрута (случайное переставление двух точек).
        /// Также реализованы разные типы мутаций:
        /// - 15% особей получают 2 мутации,
        /// - 30% — 1 мутацию,
        /// - 10% — случайное число мутаций.
        /// </summary>
        private void Mutate(Individual individual)
        {
            Random rand = new Random();
            double randomValue = rand.NextDouble();

            if (randomValue < 0.15)
            {
                SwapRandom(individual.Route, rand);
                SwapRandom(individual.Route, rand);
            }
            else if (randomValue < 0.45)
            {
                SwapRandom(individual.Route, rand);
            }
            else if (rand.NextDouble() < 0.1)
            {
                int maxMutations = Math.Max(1, cities.Count / 2);
                int mutationCount = rand.Next(1, maxMutations + 1);
                for (int m = 0; m < mutationCount; m++) SwapRandom(individual.Route, rand);
            }

            individual.Fitness = CalculateFitness(individual.Route);
        }

        /// <summary>
        /// Перестановка двух случайных элементов в маршруте.
        /// </summary>
        private void SwapRandom(int[] route, Random rand)
        {
            int i = rand.Next(cities.Count);
            int j = rand.Next(cities.Count);
            int temp = route[i];
            route[i] = route[j];
            route[j] = temp;
        }

        /// <summary>
        /// Вычисляет длину маршрута (целевая функция).
        /// </summary>
        private double CalculateFitness(int[] route)
        {
            double distance = 0;
            for (int i = 0; i < cities.Count - 1; i++)
                distance += CalculateDistance(cities[route[i]], cities[route[i + 1]]);
            distance += CalculateDistance(cities[route[cities.Count - 1]], cities[route[0]]);
            return distance;
        }

        /// <summary>
        /// Расстояние между двумя точками.
        /// </summary>
        private double CalculateDistance(Point p1, Point p2) =>
            Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        /// <summary>
        /// Отрисовка лучших маршрутов и самого лучшего.
        /// Вызывается из фонового потока через Dispatcher.
        /// </summary>
        private void DrawPopulation()
        {
            RouteCanvas.Children.Clear();
            DrawGrid();
            DrawCities();

            var topIndividuals = population.OrderBy(ind => ind.Fitness).Take(21).ToList();
            if (topIndividuals.Contains(bestIndividual)) topIndividuals.Remove(bestIndividual);

            foreach (var individual in topIndividuals.Take(20))
                DrawRoute(individual.Route, Brushes.Gray, 1, 0.2);

            DrawRoute(bestIndividual.Route, Brushes.Blue, 2, 1.0);
        }

        /// <summary>
        /// Отрисовка одного маршрута на канвасе.
        /// </summary>
        private void DrawRoute(int[] route, Brush color, int thickness, double opacity)
        {
            for (int i = 0; i < cities.Count - 1; i++)
            {
                var line = new Line
                {
                    X1 = cities[route[i]].X,
                    Y1 = cities[route[i]].Y,
                    X2 = cities[route[i + 1]].X,
                    Y2 = cities[route[i + 1]].Y,
                    Stroke = color,
                    StrokeThickness = thickness,
                    Opacity = opacity
                };
                RouteCanvas.Children.Add(line);
            }

            var closingLine = new Line
            {
                X1 = cities[route[^1]].X,
                Y1 = cities[route[^1]].Y,
                X2 = cities[route[0]].X,
                Y2 = cities[route[0]].Y,
                Stroke = color,
                StrokeThickness = thickness,
                Opacity = opacity
            };
            RouteCanvas.Children.Add(closingLine);
        }
    }

    /// <summary>
    /// Класс, представляющий одну особь (маршрут).
    /// </summary>
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