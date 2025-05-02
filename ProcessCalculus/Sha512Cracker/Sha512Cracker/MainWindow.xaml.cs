using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        // Список хэшей по умолчанию, загружаемых в текстовое поле при старте
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
        // Секундомер
        private Stopwatch stopwatch;
        // Задача для обновления времени
        private Task timerTask;
        // Токен отмены для таймера
        private CancellationTokenSource timerCts;
        // Задача для обновления прогресса UI
        private Task progressUpdateTask;
        // Токен отмены для задачи прогресса
        private CancellationTokenSource progressCts;
        // Потокобезопасная коллекция для текущих паролей
        private ConcurrentDictionary<int, string> currentPasswords;
        // Потокобезопасная коллекция для текущих индексов
        private ConcurrentDictionary<int, long> currentIndexes;
        // Флаг, указывающий, приостановлена ли работа
        private bool isPaused;
        // Сохраненное время из файла прогресса
        private long savedElapsedTicks;

        public MainWindow()
        {
            InitializeComponent();
            // Загружаем хэши по умолчанию в текстовое поле
            HashesTextBox.Text = string.Join(Environment.NewLine, DefaultHashes);
            // Инициализируем секундомер
            stopwatch = new Stopwatch();
            // Загружаем сохраненный прогресс и результаты
            LoadProgress();
            LoadResults();
        }

        // Загрузка сохраненного прогресса из файла
        private void LoadProgress()
        {
            if (File.Exists("progress.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines("progress.txt");
                    currentPasswords = new ConcurrentDictionary<int, string>();
                    currentIndexes = new ConcurrentDictionary<int, long>();
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Elapsed Time:"))
                        {
                            savedElapsedTicks = long.Parse(line.Split(':')[1].Trim());
                            TimeSpan savedTime = TimeSpan.FromTicks(savedElapsedTicks);
                            TimerTextBlock.Text = $"Прошедшее время: {savedTime:hh\\:mm\\:ss}";
                        }
                        else if (line.StartsWith("Поток ") && line.Contains(":"))
                        {
                            // Пример строки: "Поток 1: abcdefg:123456"
                            string[] parts = line.Split(':');
                            if (parts.Length >= 3)
                            {
                                int threadId = int.Parse(parts[0].Replace("Поток ", "").Trim()) - 1;
                                string password = parts[1].Trim();
                                long index = long.Parse(parts[2].Trim());
                                currentPasswords.TryAdd(threadId, password);
                                currentIndexes.TryAdd(threadId, index);
                            }
                        }
                    }
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
            if (File.Exists("results.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines("results.txt");
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            ResultsTextBox.AppendText($"{line}\n");
                            // Добавляем хэш в foundHashes для учета
                            string[] parts = line.Split(':');
                            if (parts.Length == 2)
                            {
                                foundHashes.Add(parts[0].Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"Ошибка загрузки результатов: {ex.Message}\n");
                }
            }
        }

        // Обновление времени в UI
        private async Task UpdateTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Dispatcher.Invoke(() =>
                {
                    TimeSpan elapsed = stopwatch.Elapsed + TimeSpan.FromTicks(savedElapsedTicks);
                    TimerTextBlock.Text = $"Прошедшее время: {elapsed:hh\\:mm\\:ss}";
                });
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        // Обновление прогресса потоков в UI
        private async Task UpdateProgressAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Dispatcher.Invoke(() =>
                {
                    ThreadProgressListBox.Items.Clear();
                    foreach (var kvp in currentPasswords.OrderBy(k => k.Key))
                    {
                        ThreadProgressListBox.Items.Add($"Поток {kvp.Key + 1}: {kvp.Value}");
                    }
                });
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        // Обработчик нажатия кнопки "Начать"
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем хэши из текстового поля, разделяя по строкам
            targetHashes = HashesTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // Если прогресс не загружен, инициализируем коллекции
            if (currentPasswords == null)
            {
                currentPasswords = new ConcurrentDictionary<int, string>();
            }
            if (currentIndexes == null)
            {
                currentIndexes = new ConcurrentDictionary<int, long>();
            }
            // Очищаем поле результатов, если это новый запуск
            if (!isPaused)
            {
                ResultsTextBox.Clear();
                LoadResults(); // Перезагружаем результаты
            }
            // Обновляем счетчик найденных хэшей
            FoundCountTextBlock.Text = $"Найдено: {foundHashes.Count}/{targetHashes.Count}";
            // Отключаем кнопку "Начать"
            StartButton.IsEnabled = false;
            // Включаем кнопку "Пауза/Возобновить"
            PauseResumeButton.IsEnabled = true;
            // Включаем кнопку "Сброс"
            ResetButton.IsEnabled = true;
            // Создаем новый источник токена отмены для задач
            cts = new CancellationTokenSource();
            // Создаем новый источник токена отмены для таймера
            timerCts = new CancellationTokenSource();
            // Создаем новый источник токена отмены для прогресса
            progressCts = new CancellationTokenSource();
            // Запускаем секундомер
            stopwatch.Restart();
            // Запускаем задачу обновления времени
            timerTask = UpdateTimerAsync(timerCts.Token);
            // Запускаем задачу обновления прогресса
            progressUpdateTask = UpdateProgressAsync(progressCts.Token);
            // Сбрасываем флаг паузы
            isPaused = false;
            // Устанавливаем текст кнопки на "Пауза"
            PauseResumeButton.Content = "Пауза";

            try
            {
                // Запускаем процесс взлома асинхронно
                await RunCrackerAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Если задача была отменена, выводим сообщение
                ResultsTextBox.AppendText("Взлом приостановлен.\n");
            }
            finally
            {
                // Очищаем ресурсы после завершения
                Cleanup();
            }
        }

        // Обработчик нажатия кнопки "Пауза/Возобновить"
        private async void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPaused)
            {
                // Если не на паузе, отменяем задачи и таймер
                cts.Cancel();
                timerCts.Cancel();
                progressCts.Cancel();
                // Останавливаем секундомер
                stopwatch.Stop();
                // Сохраняем прогресс в файл
                SaveProgress();
                // Меняем текст кнопки на "Возобновить"
                PauseResumeButton.Content = "Возобновить";
                // Устанавливаем флаг паузы
                isPaused = true;
                // Кнопка "Начать" остается неактивной
            }
            else
            {
                // Если на паузе, возобновляем работу
                cts = new CancellationTokenSource();
                timerCts = new CancellationTokenSource();
                progressCts = new CancellationTokenSource();
                stopwatch.Start();
                timerTask = UpdateTimerAsync(timerCts.Token);
                progressUpdateTask = UpdateProgressAsync(progressCts.Token);
                PauseResumeButton.Content = "Пауза";
                isPaused = false;
                // Кнопка "Начать" остается неактивной

                try
                {
                    // Возобновляем процесс взлома
                    await RunCrackerAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    ResultsTextBox.AppendText("Взлом приостановлен.\n");
                }
                finally
                {
                    Cleanup();
                }
            }
        }

        // Обработчик нажатия кнопки "Сброс"
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Если взлом активен, отменяем задачи
            if (stopwatch.IsRunning)
            {
                cts?.Cancel();
                timerCts?.Cancel();
                progressCts?.Cancel();
                stopwatch.Stop();
            }
            // Очищаем прогресс
            foundHashes.Clear();
            currentPasswords.Clear();
            currentIndexes.Clear();
            ResultsTextBox.Clear();
            ThreadProgressListBox.Items.Clear();
            FoundCountTextBlock.Text = $"Найдено: 0/{targetHashes?.Count ?? 0}";
            TimerTextBlock.Text = "Прошедшее время: 00:00:00";
            savedElapsedTicks = 0;
            // Удаляем файлы прогресса и результатов, если они существуют
            if (File.Exists("progress.txt"))
            {
                try
                {
                    File.Delete("progress.txt");
                    ResultsTextBox.AppendText("Файл прогресса удален.\n");
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"Ошибка удаления файла прогресса: {ex.Message}\n");
                }
            }
            if (File.Exists("results.txt"))
            {
                try
                {
                    File.Delete("results.txt");
                    ResultsTextBox.AppendText("Файл результатов удален.\n");
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"Ошибка удаления файла результатов: {ex.Message}\n");
                }
            }
            // Восстанавливаем состояние кнопок
            StartButton.IsEnabled = true;
            PauseResumeButton.IsEnabled = false;
            PauseResumeButton.Content = "Пауза";
            ResetButton.IsEnabled = true;
            isPaused = false;
        }

        // Запуска взлома
        private async Task RunCrackerAsync(CancellationToken token)
        {
            // Вычисляем общее количество комбинаций паролей
            long totalCombinations = (long)Math.Pow(Charset.Length, PasswordLength);
            // Определяем количество потоков на основе числа процессоров
            int numThreads = Environment.ProcessorCount;
            // Вычисляем размер блока комбинаций для каждого потока
            long chunkSize = totalCombinations / numThreads;
            // Создаем массив задач
            Task[] tasks = new Task[numThreads];

            // Запускаем задачи для каждого потока
            for (int i = 0; i < numThreads; i++)
            {
                // Определяем начало и конец диапазона для потока
                long start = i * chunkSize;
                long end = (i == numThreads - 1) ? totalCombinations : start + chunkSize;
                // Если есть сохраненный индекс, начинаем с него
                long savedIndex = currentIndexes.ContainsKey(i) ? currentIndexes[i] : start;
                start = Math.Max(start, savedIndex);
                int threadId = i;
                // Создаем задачу, которая выполняет взлом в указанном диапазоне
                tasks[i] = Task.Run(() => Crack(start, end, targetHashes.ToArray(), threadId, token), token);
                // Task.Run запускает метод Crack в отдельном потоке
                // Передаем threadId для идентификации потока в коллекции currentPasswords.
                // Токен отмены позволяет остановить задачу при паузе.
            }

            // Ожидаем завершения всех задач
            await Task.WhenAll(tasks);
            // Пояснение: Task.WhenAll возвращает управление, когда все задачи завершены
            // или выброшено исключение (например, при отмене через token).

            // Проверяем, найдены ли все хэши
            if (foundHashes.Count < targetHashes.Count)
            {
                foreach (var hash in targetHashes.Where(h => !foundHashes.Contains(h)))
                {
                    Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Пароль не найден для {hash}.\n"));
                }
            }
        }

        // Метод взлома для указанного диапазона комбинаций
        private void Crack(long start, long end, string[] hashesToCrack, int threadId, CancellationToken token)
        {
            long charsetSize = Charset.Length;

            // Перебираем комбинации в заданном диапазоне
            for (long i = start; i < end; i++)
            {
                // Проверяем, не была ли задача отменена
                token.ThrowIfCancellationRequested();
                // Обновляем текущий индекс потока
                currentIndexes.AddOrUpdate(threadId, i, (key, oldValue) => i);

                // Генерируем пароль
                StringBuilder password = new StringBuilder();
                long temp = i;
                for (int j = 0; j < PasswordLength; j++)
                {
                    password.Append(Charset[(int)(temp % charsetSize)]);
                    temp /= charsetSize;
                }

                // Получаем итоговый пароль
                string finalPassword = password.ToString();
                // Обновляем текущий пароль потока
                currentPasswords.AddOrUpdate(threadId, finalPassword, (key, oldValue) => finalPassword);

                // Вычисляем хэш пароля
                string currentHash = CalculateSha512(finalPassword);
                foreach (string targetHash in hashesToCrack)
                {
                    if (!foundHashes.Contains(targetHash) && currentHash == targetHash)
                    {
                        // Сохраняем результат в файл
                        SaveResult(targetHash, finalPassword);
                        // Обновляем UI
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

                // Если найдены все хэши, завершаем работу
                if (foundHashes.Count == hashesToCrack.Length)
                {
                    return;
                }
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

        // Сохранение прогресса в файл
        private void SaveProgress()
        {
            var progress = new StringBuilder();
            progress.AppendLine($"Elapsed Time: {stopwatch.ElapsedTicks + savedElapsedTicks}");
            progress.AppendLine("Текущие пароли:");
            foreach (var kvp in currentPasswords.OrderBy(k => k.Key))
            {
                long index = currentIndexes.ContainsKey(kvp.Key) ? currentIndexes[kvp.Key] : 0;
                progress.AppendLine($"Поток {kvp.Key + 1}: {kvp.Value}:{index}");
            }
            File.WriteAllText("progress.txt", progress.ToString());
        }

        // Сохранение результата в файл
        private void SaveResult(string hash, string password)
        {
            try
            {
                File.AppendAllText("results.txt", $"{hash}: {password}\n");
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ResultsTextBox.AppendText($"Ошибка сохранения результата: {ex.Message}\n"));
            }
        }

        // Обработчик закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                cts?.Cancel();
                timerCts?.Cancel();
                progressCts?.Cancel();
                SaveProgress();
            }
        }

        // Очистка ресурсов после завершения взлома
        private void Cleanup()
        {
            stopwatch.Stop();
            timerCts?.Cancel();
            timerTask?.Wait();
            progressCts?.Cancel();
            progressUpdateTask?.Wait();
            cts?.Dispose();
            timerCts?.Dispose();
            timerCts = null;
            progressCts?.Dispose();
            progressCts = null;
            if (!isPaused)
            {
                StartButton.IsEnabled = true;
                PauseResumeButton.IsEnabled = false;
                PauseResumeButton.Content = "Пауза";
            }
        }
    }
}