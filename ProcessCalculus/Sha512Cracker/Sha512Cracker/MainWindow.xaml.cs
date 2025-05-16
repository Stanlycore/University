using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Org.BouncyCastle.Crypto.Digests;
using System.IO;
using System.Diagnostics;

namespace Sha512CrackerWpfApp
{
    public partial class MainWindow : Window
    {
        // Путь для сохранения файлов
        private static readonly string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sha512Cracker");
        private static readonly string ProgressFile = Path.Combine(SavePath, "progress.txt");
        private static readonly string ResultsFile = Path.Combine(SavePath, "results.txt");
        private static readonly string PerformanceFile = Path.Combine(SavePath, "performance.txt");

        // Список хэшей по умолчанию
        private static readonly string[] DefaultHashes = {
            "67705692cb50fbb6921e86e0429b744bbf06cd345eae02ae471b961b1fac2cf413af10eba03cc3e0a66ae05f305c7cd5172208245c7f6e8a1446bb0c5dbc8afd",
            "0678e4bde9816012477bc96e70cc7e5b419b206af07279ff88a564d8744a54ffb5c8092df53ae75c5d8a0445e85bf9bccb927bd34b2f373a90948e2c7d856808",
            "2ce1c01d6ba106e375a8fd665d9323abadd18d33e8930aadc3a8166f9491fbb67d223d5a5174de25b99b39f54eab38623e981271f55465b354a5e296d36a8354",
            "0ca82ffc3b1f17064ad1ebbae0fec9db55b1bb727bbf9a4f8cfaa8d6485063baf31257d120205699a26ac467d50e8eb872c7acbfd9c728e4c68d5b7506800ed3",
            "c8a6cd3dbcce9cde5a291d91f14c2f1fd4c3961c80b5f847a10c944fdfae2ee52b0fd31e839bbf84c90da265ff7e8d8620a4508561c2cb1d9d050119b125a2ee",
            "7a206b004e32d2e96663da86a1dda28c121736cb4bec418a184245cadcff8356050d8213eaf412edff5d47b9f1976319f5f29baec8d1a2343b7f48a844dea547",
            "54c69aaf89f7fcc483cf0397e6ca8aee61cbd1f39c6fd68680196762dcc017b229889b1976d64baebc4956ade5119af642cfc0df263b108795029ea4c7921523",
            "ccd43b4a8b905cc4d6ff11b78f85323d882b08dc08193eecde3162f0a95b129068a6c331821cdc228372bae63846d7324e249e716ebd9d8c8f743fed2ba50736",
            "349c82f534142da76321aeb46c2739310853bc20465c118bdd3aeeec21b9e35f1c0f07ef8fc7cac5589a7b239bc737c5ea5bc62d51ee9f3ac4cd1de2592ae9ad",
            "62b93fdfb944a5cfd6af30b250ca97a7b7c47561012aea9c049ef241ff78e173ce4c76f3e3d3e43d3f4844e37e7313ebdbe9f2ff636880f1ebd75353e8a55965",
            "164719442f78b7222e29d3f31ec29acab6b90c99f966583a43c14313e790e130406a3ed3b3764cf44108ace4f7fda561dca45f7f640e94122b0dc3eed4d8c3bb",
            "4fcc429d07b98352e4a2877f236b73c8ff2a62b825a54a0c82357f2c09f3caef32c53c684623c2a8631b65d1e33f1f36d7bf3aca6c474351ed1c48811cc4550c",
            "6e9564685eb16c0c516ff1210dc6e94dc183faf1a1026116fccddb4d215e8df8eb82440d2bbbf4ee5d605edef87db7ce602eeb0bf0b8000b063753b8996e2ae3",
            "fd54a8c44341a054107c8b61e0f83e7dd54a21a6de26cb5f165c7e24887a6766c0ee8c31e55c123793b18d2ab7202d135455905b588cee8796d42361cb2804cd",
            "3aaa72ccf086c65676afa0142106f9f50c2d40a4e89dc850201bb7d829e969dcce38e5050671a56cf2f3f74c56756842ac3f9a2f69b7631dd9966dad9bffe8a9",
            "ad67f180e80a6cdce0c8b4f97e999ef6404763cd0ab17dc19bd3acda45716b35f4cfd8eafd2e07596a858857caa36b0e395bfa6b55854a16750085b9c8ea06d8",
            "b5eae3c46182a31d9c32c7d469c2e2725a4506230116df86d1d3998705cf2190974147c4e6f01e9dc5b460d473746cf007b81a7f4a38c512562cc384ce7f7fa4",
            "38bfebbfafaec1f1b19504bcdbd513b240cc5b7333e8be722883a471f6e6145df7ddcb6b83bae337eb1f7deca06dd0fb2573d7e6d8f74ad3240aed807f07039a",
            "0042605631d108a4a55b6bf6616c0f28b100b0695f90a950aeddb04378188ffd450c215c59d1797030a8f432e63359623cfdf726c118bdd3aeeec21b9e35f1c0",
            "ed451f8b3026c6236d9282ceaa6b0cba69be4c80040430fb0b65e32119bdc2bfc8debdfbe530711c94550c3b947c3fab5eac0e079cf690b0b5fd6b53bb6631c9",
            "af52cba7ccb488166d3c67d935f239339744e294da8e573ae97306e1f9c20514d9a2d3bd7ccb718b6f389ede297645bc25a502174b8b2120a1f5b743dd04301d"
        };
        // Набор символов для генерации паролей
        private static readonly string Charset = "0123456789abcdefghijklmnopqrstuvwxyz";
        // Длина пароля
        private const int PasswordLength = 7;
        // Множество для хранения найденных хэшей
        private HashSet<string> foundHashes = new HashSet<string>();
        // Список целевых хэшей для взлома
        private List<string> targetHashes;
        // Объект для управления отменой задач
        private CancellationTokenSource cts;
        // Секундомер для отслеживания времени работы
        private Stopwatch stopwatch;
        // Таймеры для обновления UI и замера производительности
        private DispatcherTimer timer;
        private DispatcherTimer progressTimer;
        private DispatcherTimer performanceTimer;
        // Потокобезопасная коллекция для текущих паролей
        private ConcurrentDictionary<int, string> currentPasswords;
        // Потокобезопасная коллекция для проверенных диапазонов
        private ConcurrentBag<(long start, long end)> checkedRanges;
        // Количество потоков
        private int numThreads;
        // Флаг, указывающий, приостановлена ли работа
        private bool isPaused;
        // Сохраненное время из файла прогресса
        private long savedElapsedTicks;
        // Счетчик проверенных паролей
        private long passwordsChecked;
        // Начальное значение проверенных паролей для замера
        private long startPasswordsChecked;

        public MainWindow()
        {
            InitializeComponent();
            // Загружаем хэши по умолчанию в текстовое поле
            HashesTextBox.Text = string.Join(Environment.NewLine, DefaultHashes);
            // Инициализируем секундомер
            stopwatch = new Stopwatch();
            // Создаем папку для сохранения файлов
            try
            {
                Directory.CreateDirectory(SavePath);
                ResultsTextBox.AppendText($"Папка для файлов создана: {SavePath}\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Ошибка создания папки {SavePath}: {ex.Message}\n");
            }
            // Загружаем сохраненный прогресс и результаты
            LoadProgress();
            LoadResults();
        }

        // Загрузка сохраненного прогресса из файла
        private void LoadProgress()
        {
            if (File.Exists(ProgressFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(ProgressFile);
                    currentPasswords = new ConcurrentDictionary<int, string>();
                    checkedRanges = new ConcurrentBag<(long start, long end)>();
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Elapsed Time:"))
                        {
                            savedElapsedTicks = long.Parse(line.Split(':')[1].Trim());
                            TimeSpan savedTime = TimeSpan.FromTicks(savedElapsedTicks);
                            TimerTextBlock.Text = $"Прошедшее время: {savedTime:hh\\:mm\\:ss}";
                        }
                        else if (line.StartsWith("Threads:"))
                        {
                            numThreads = int.Parse(line.Split(':')[1].Trim());
                            ThreadCountTextBox.Text = numThreads.ToString();
                        }
                        else if (line.StartsWith("Range:"))
                        {
                            string[] parts = line.Replace("Range:", "").Split('-');
                            long start = long.Parse(parts[0].Trim());
                            long end = long.Parse(parts[1].Trim());
                            checkedRanges.Add((start, end));
                        }
                        else if (line.StartsWith("Поток ") && line.Contains(":"))
                        {
                            string[] parts = line.Split(':');
                            int threadId = int.Parse(parts[0].Replace("Поток ", "").Trim()) - 1;
                            string password = parts[1].Trim();
                            currentPasswords.TryAdd(threadId, password);
                        }
                    }
                    ResultsTextBox.AppendText($"Прогресс загружен из {ProgressFile}\n");
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"Ошибка загрузки прогресса: {ex.Message}\n");
                }
            }
        }

        // Загрузка сохраненных результатов из файла
        private void LoadResults()
        {
            if (File.Exists(ResultsFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(ResultsFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            ResultsTextBox.AppendText($"{line}\n");
                            string[] parts = line.Split(':');
                            if (parts.Length == 2)
                            {
                                foundHashes.Add(parts[0].Trim());
                            }
                        }
                    }
                    ResultsTextBox.AppendText($"Результаты загружены из {ResultsFile}\n");
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"Ошибка загрузки результатов: {ex.Message}\n");
                }
            }
        }

        // Обновление времени в UI
        private void UpdateTimer(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed + TimeSpan.FromTicks(savedElapsedTicks);
            TimerTextBlock.Text = $"Прошедшее время: {elapsed:hh\\:mm\\:ss}";
        }

        // Обновление прогресса потоков в UI
        private void UpdateProgress(object sender, EventArgs e)
        {
            ThreadProgressListBox.Items.Clear();
            foreach (var kvp in currentPasswords.OrderBy(k => k.Key))
            {
                ThreadProgressListBox.Items.Add($"Поток {kvp.Key + 1}: {kvp.Value}");
            }
        }

        // Замер производительности
        private void MeasurePerformance(object sender, EventArgs e)
        {
            int minute = (int)(stopwatch.Elapsed.TotalMinutes + 1);
            if (minute > 5)
            {
                performanceTimer.Stop();
                return;
            }

            long currentChecked = passwordsChecked - startPasswordsChecked;
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Измерение производительности ({DateTime.Now:yyyy-MM-dd HH:mm:ss}, Потоков: {numThreads}):");
                sb.AppendLine($"{minute} минута: {currentChecked} паролей проверено");
                File.AppendAllText(PerformanceFile, sb.ToString());
                ResultsTextBox.AppendText($"Производительность за {minute} минуту сохранена в {PerformanceFile}\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Ошибка записи производительности: {ex.Message}\n");
            }
        }

        // Проверка корректности количества потоков
        private bool ValidateThreadCount(out int threadCount)
        {
            if (!int.TryParse(ThreadCountTextBox.Text, out threadCount))
            {
                ResultsTextBox.AppendText("Ошибка: Введите корректное число потоков.\n");
                return false;
            }
            if (threadCount < 1 || threadCount > Environment.ProcessorCount)
            {
                ResultsTextBox.AppendText($"Ошибка: Число потоков должно быть от 1 до {Environment.ProcessorCount}.\n");
                return false;
            }
            return true;
        }

        // Обработчик нажатия кнопки "Начать"
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateThreadCount(out numThreads))
                return;

            // Настраиваем ThreadPool
            ThreadPool.SetMaxThreads(numThreads + 2, numThreads + 2);
            StartCracking();
        }

        // Запуск процесса взлома
        private void StartCracking()
        {
            // Получаем хэши из текстового поля
            targetHashes = HashesTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // Очищаем и инициализируем коллекции
            currentPasswords = new ConcurrentDictionary<int, string>();
            if (checkedRanges == null)
                checkedRanges = new ConcurrentBag<(long start, long end)>();
            for (int i = 0; i < numThreads; i++)
                currentPasswords.TryAdd(i, "Инициализация...");
            // Очищаем поле результатов, если это новый запуск
            if (!isPaused)
            {
                ResultsTextBox.Clear();
                LoadResults();
            }
            // Обновляем счетчик найденных хэшей
            FoundCountTextBlock.Text = $"Найдено: {foundHashes.Count}/{targetHashes.Count}";
            // Отключаем кнопку "Начать"
            StartButton.IsEnabled = false;
            // Включаем кнопки
            PauseResumeButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
            // Создаем новый источник токена отмены
            cts = new CancellationTokenSource();
            // Сбрасываем счетчик паролей
            passwordsChecked = 0;
            startPasswordsChecked = 0;
            // Запускаем секундомер
            stopwatch.Restart();
            // Запускаем таймеры
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += UpdateTimer;
            timer.Start();
            progressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            progressTimer.Tick += UpdateProgress;
            progressTimer.Start();
            performanceTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            performanceTimer.Tick += MeasurePerformance;
            performanceTimer.Start();
            // Сбрасываем флаг паузы
            isPaused = false;
            PauseResumeButton.Content = "Пауза";

            // Запускаем взлом
            RunCrackerAsync(cts.Token);
        }

        // Обработчик нажатия кнопки "Пауза/Возобновить"
        private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPaused)
            {
                cts.Cancel();
                timer.Stop();
                progressTimer.Stop();
                performanceTimer.Stop();
                stopwatch.Stop();
                SaveProgress();
                PauseResumeButton.Content = "Возобновить";
                isPaused = true;
            }
            else
            {
                if (!ValidateThreadCount(out numThreads))
                    return;

                // Настраиваем ThreadPool
                ThreadPool.SetMaxThreads(numThreads + 2, numThreads + 2);
                cts = new CancellationTokenSource();
                // Очищаем и инициализируем currentPasswords
                currentPasswords = new ConcurrentDictionary<int, string>();
                for (int i = 0; i < numThreads; i++)
                    currentPasswords.TryAdd(i, "Инициализация...");
                stopwatch.Start();
                timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += UpdateTimer;
                timer.Start();
                progressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                progressTimer.Tick += UpdateProgress;
                progressTimer.Start();
                performanceTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
                performanceTimer.Tick += MeasurePerformance;
                performanceTimer.Start();
                PauseResumeButton.Content = "Пауза";
                isPaused = false;
                RunCrackerAsync(cts.Token);
            }
        }

        // Обработчик нажатия кнопки "Сброс"
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateThreadCount(out numThreads))
                return;

            if (stopwatch.IsRunning)
            {
                cts?.Cancel();
                timer?.Stop();
                progressTimer?.Stop();
                performanceTimer?.Stop();
                stopwatch.Stop();
            }
            foundHashes.Clear();
            currentPasswords = new ConcurrentDictionary<int, string>();
            for (int i = 0; i < numThreads; i++)
                currentPasswords.TryAdd(i, "Инициализация...");
            checkedRanges = new ConcurrentBag<(long start, long end)>();
            ResultsTextBox.Clear();
            ThreadProgressListBox.Items.Clear();
            FoundCountTextBlock.Text = $"Найдено: 0/{targetHashes?.Count ?? 0}";
            TimerTextBlock.Text = "Прошедшее время: 00:00:00";
            savedElapsedTicks = 0;
            passwordsChecked = 0;
            startPasswordsChecked = 0;
            try
            {
                if (File.Exists(ProgressFile))
                    File.Delete(ProgressFile);
                if (File.Exists(ResultsFile))
                    File.Delete(ResultsFile);
                ResultsTextBox.AppendText("Файлы прогресса и результатов удалены.\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Ошибка удаления файлов: {ex.Message}\n");
            }
            StartButton.IsEnabled = true;
            PauseResumeButton.IsEnabled = false;
            PauseResumeButton.Content = "Пауза";
            ResetButton.IsEnabled = true;
            isPaused = false;
        }

        // Запуск взлома через ThreadPool
        private void RunCrackerAsync(CancellationToken token)
        {
            long totalCombinations = (long)Math.Pow(Charset.Length, PasswordLength);
            long chunkSize = totalCombinations / numThreads;
            int completedThreads = 0;

            for (int i = 0; i < numThreads; i++)
            {
                long start = i * chunkSize;
                long end = (i == numThreads - 1) ? totalCombinations : start + chunkSize;
                int threadId = i;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    Crack(start, end, targetHashes.ToArray(), threadId, token);
                    Interlocked.Increment(ref completedThreads);
                });
            }

            // Проверяем завершение всех потоков
            Task.Run(() =>
            {
                while (completedThreads < numThreads && !token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                if (!token.IsCancellationRequested && foundHashes.Count < targetHashes.Count)
                {
                    foreach (var hash in targetHashes.Where(h => !foundHashes.Contains(h)))
                    {
                        Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Пароль не найден для {hash}.\n"));
                    }
                }
                Dispatcher.Invoke(() => Cleanup());
            });
        }

        // Метод взлома для указанного диапазона комбинаций
        private void Crack(long start, long end, string[] hashesToCrack, int threadId, CancellationToken token)
        {
            long charsetSize = Charset.Length;
            long chunkSize = 1000;
            long current = start;

            while (current < end && !token.IsCancellationRequested)
            {
                long chunkEnd = Math.Min(current + chunkSize, end);
                bool isChecked = checkedRanges.Any(range => current >= range.start && current <= range.end);
                if (!isChecked)
                {
                    for (long i = current; i < chunkEnd; i++)
                    {
                        if (token.IsCancellationRequested)
                            break;

                        StringBuilder password = new StringBuilder();
                        long temp = i;
                        for (int j = 0; j < PasswordLength; j++)
                        {
                            password.Append(Charset[(int)(temp % charsetSize)]);
                            temp /= charsetSize;
                        }
                        string finalPassword = password.ToString();
                        currentPasswords.AddOrUpdate(threadId, finalPassword, (key, oldValue) => finalPassword);

                        string currentHash = CalculateSha512(finalPassword);
                        foreach (string targetHash in hashesToCrack)
                        {
                            if (!foundHashes.Contains(targetHash) && currentHash == targetHash)
                            {
                                SaveResult(targetHash, finalPassword);
                                Dispatcher.Invoke(() =>
                                {
                                    ResultsTextBox.AppendText($"Пароль найден для {targetHash}: {finalPassword}\n");
                                    FoundCountTextBlock.Text = $"Найдено: {foundHashes.Count + 1}/{targetHashes.Count}";
                                });
                                lock (foundHashes)
                                {
                                    foundHashes.Add(targetHash);
                                }
                                break;
                            }
                        }
                        Interlocked.Increment(ref passwordsChecked);
                        if (foundHashes.Count == hashesToCrack.Length)
                            return;
                    }
                    checkedRanges.Add((current, chunkEnd - 1));
                }
                current = chunkEnd;
            }
        }

        // Метод вычисления SHA-512 хэша
        private string CalculateSha512(string input)
        {
            // Создаем новый экземпляр объекта Sha512Digest из библиотеки BouncyCastle для вычисления SHA-512 хэша
            Sha512Digest digest = new Sha512Digest();
            // Преобразуем входную строку (пароль) в массив байтов, используя кодировку UTF-8
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            // Передаем массив байтов в алгоритм хэширования, указывая начальную позицию (0) и длину массива
            digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
            // Создаем массив байтов для хранения результата хэша, размер которого определяется методом GetDigestSize
            byte[] hashBytes = new byte[digest.GetDigestSize()];
            // Завершаем процесс хэширования и записываем результат в массив hashBytes, начиная с позиции 0
            digest.DoFinal(hashBytes, 0);
            // Создаем StringBuilder для построения строки шестнадцатеричного представления хэша
            StringBuilder sb = new StringBuilder();
            // Проходим по каждому байту в массиве хэша
            for (int i = 0; i < hashBytes.Length; i++)
            {
                // Преобразуем байт в строку в шестнадцатеричном формате (две цифры, строчные буквы) и добавляем в StringBuilder
                sb.Append(hashBytes[i].ToString("x2"));
            }
            // Возвращаем итоговую строку хэша в виде шестнадцатеричного представления
            return sb.ToString();
        }

        // Объединение соседних диапазонов
        private List<(long start, long end)> MergeRanges(IEnumerable<(long start, long end)> ranges)
        {
            // Сортируем диапазоны по началу
            var sortedRanges = ranges.OrderBy(r => r.start).ToList();
            if (!sortedRanges.Any())
                return new List<(long start, long end)>();

            var merged = new List<(long start, long end)>();
            var current = sortedRanges[0];

            // Проходим по отсортированным диапазонам
            for (int i = 1; i < sortedRanges.Count; i++)
            {
                var next = sortedRanges[i];
                // Если следующий диапазон начинается сразу после текущего, объединяем
                if (current.end + 1 >= next.start)
                {
                    current = (current.start, Math.Max(current.end, next.end));
                }
                else
                {
                    // Если диапазоны не соседние, сохраняем текущий и переходим к следующему
                    merged.Add(current);
                    current = next;
                }
            }
            // Добавляем последний диапазон
            merged.Add(current);

            return merged;
        }

        // Сохранение прогресса в файл с объединением диапазонов
        private void SaveProgress()
        {
            try
            {
                var progress = new StringBuilder();
                progress.AppendLine($"Elapsed Time: {stopwatch.ElapsedTicks + savedElapsedTicks}");
                progress.AppendLine($"Threads: {numThreads}");
                progress.AppendLine("Проверенные диапазоны:");

                // Объединяем диапазоны
                var mergedRanges = MergeRanges(checkedRanges);
                foreach (var range in mergedRanges)
                {
                    progress.AppendLine($"Range:{range.start}-{range.end}");
                }

                progress.AppendLine("Текущие пароли:");
                foreach (var kvp in currentPasswords.OrderBy(k => k.Key))
                {
                    progress.AppendLine($"Поток {kvp.Key + 1}:{kvp.Value}");
                }
                File.WriteAllText(ProgressFile, progress.ToString());

                // Обновляем checkedRanges объединенными диапазонами
                checkedRanges = new ConcurrentBag<(long start, long end)>(mergedRanges);

                Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Прогресс сохранен в {ProgressFile} с объединенными диапазонами\n"));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Ошибка сохранения прогресса: {ex.Message}\n"));
            }
        }

        // Сохранение результата в файл
        private void SaveResult(string hash, string password)
        {
            try
            {
                File.AppendAllText(ResultsFile, $"{hash}: {password}\n");
                Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Результат сохранен в {ResultsFile}: {hash}: {password}\n"));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Ошибка сохранения результата: {ex.Message}\n"));
            }
        }

        // Обработчик при закрытии окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                cts?.Cancel();
                timer?.Stop();
                progressTimer?.Stop();
                performanceTimer?.Stop();
                SaveProgress();
            }
        }

        // Очистка после завершения поиска
        private void Cleanup()
        {
            stopwatch.Stop();
            timer?.Stop();
            progressTimer?.Stop();
            performanceTimer?.Stop();
            cts?.Dispose();
            cts = null;
            if (!isPaused)
            {
                StartButton.IsEnabled = true;
                PauseResumeButton.IsEnabled = false;
                PauseResumeButton.Content = "Пауза";
            }
        }
    }
}