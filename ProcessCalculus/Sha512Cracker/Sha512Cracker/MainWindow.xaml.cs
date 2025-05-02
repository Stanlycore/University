using System;
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

        // Текущий алфавит для генерации паролей
        private string charset;
        // Текущая длина пароля
        private int passwordLength;
        // Множество для хранения найденных хэшей
        private HashSet<string> foundHashes = new HashSet<string>();
        // Список целевых хэшей для взлома
        private List<string> targetHashes;
        // Объект для управления отменой задач
        private CancellationTokenSource cts;
        // Секундомер для отслеживания времени работы
        private Stopwatch stopwatch;
        // Задача для обновления времени
        private Task timerTask;
        // Токен отмены для таймера
        private CancellationTokenSource timerCts;
        // Текущий пароль
        private string currentPassword = "Нет данных";
        // Текущий индекс комбинации
        private long currentIndex;
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
            // Инициализируем начальные значения алфавита и длины пароля
            charset = CharsetTextBox.Text;
            passwordLength = int.TryParse(PasswordLengthTextBox.Text, out int length) ? length : 7;
        }

        // Обработчик нажатия кнопки "Подтвердить" для алфавита и длины пароля
        private void ConfirmSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем и сохраняем алфавит
            string newCharset = CharsetTextBox.Text;
            if (string.IsNullOrWhiteSpace(newCharset))
            {
                ResultsTextBox.AppendText("Ошибка: алфавит не может быть пустым.\n");
                return;
            }
            charset = newCharset;

            // Проверяем и сохраняем длину пароля
            if (!int.TryParse(PasswordLengthTextBox.Text, out int newLength) || newLength <= 0)
            {
                ResultsTextBox.AppendText("Ошибка: длина пароля должна быть положительным числом.\n");
                return;
            }
            passwordLength = newLength;

            ResultsTextBox.AppendText($"Установлены алфавит: {charset}, длина пароля: {passwordLength}\n");
        }

        // Загрузка сохраненного прогресса из файла
        private void LoadProgress()
        {
            if (File.Exists("progress.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines("progress.txt");
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Elapsed Time:"))
                        {
                            savedElapsedTicks = long.Parse(line.Split(':')[1].Trim());
                            TimeSpan savedTime = TimeSpan.FromTicks(savedElapsedTicks);
                            TimerTextBlock.Text = $"Прошедшее время: {savedTime:hh\\:mm\\:ss}";
                        }
                        else if (line.StartsWith("Пароль:") && line.Contains(":"))
                        {
                            string[] parts = line.Split(':');
                            if (parts.Length >= 3)
                            {
                                currentPassword = parts[1].Trim();
                                currentIndex = long.Parse(parts[2].Trim());
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

        // Асинхронное обновление времени в UI
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

        // Асинхронное обновление прогресса в UI
        private async Task UpdateProgressAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Dispatcher.Invoke(() =>
                {
                    ThreadProgressListBox.Items.Clear();
                    ThreadProgressListBox.Items.Add($"Пароль: {currentPassword}");
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
            // Проверяем, установлен ли алфавит и длина пароля
            if (string.IsNullOrEmpty(charset) || passwordLength <= 0)
            {
                ResultsTextBox.AppendText("Ошибка: необходимо подтвердить алфавит и длину пароля.\n");
                return;
            }

            // Получаем хэши из текстового поля
            targetHashes = HashesTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
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
            // Включаем кнопку "Пауза/Возобновить"
            PauseResumeButton.IsEnabled = true;
            // Включаем кнопку "Сброс"
            ResetButton.IsEnabled = true;
            // Создаем новый источник токена отмены
            cts = new CancellationTokenSource();
            timerCts = new CancellationTokenSource();
            // Запускаем секундомер
            stopwatch.Restart();
            // Запускаем задачу обновления времени
            timerTask = UpdateTimerAsync(timerCts.Token);
            // Запускаем задачу обновления прогресса
            Task progressTask = UpdateProgressAsync(timerCts.Token);
            // Сбрасываем флаг паузы
            isPaused = false;
            PauseResumeButton.Content = "Пауза";

            try
            {
                // Запускаем процесс взлома асинхронно в одном потоке
                await Task.Run(() => Crack(0, (long)Math.Pow(charset.Length, passwordLength), targetHashes.ToArray(), cts.Token), cts.Token);
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

        // Обработчик нажатия кнопки "Пауза/Возобновить"
        private async void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPaused)
            {
                cts.Cancel();
                timerCts.Cancel();
                stopwatch.Stop();
                SaveProgress();
                PauseResumeButton.Content = "Возобновить";
                isPaused = true;
            }
            else
            {
                cts = new CancellationTokenSource();
                timerCts = new CancellationTokenSource();
                stopwatch.Start();
                timerTask = UpdateTimerAsync(timerCts.Token);
                Task progressTask = UpdateProgressAsync(timerCts.Token);
                PauseResumeButton.Content = "Пауза";
                isPaused = false;

                try
                {
                    await Task.Run(() => Crack(currentIndex, (long)Math.Pow(charset.Length, passwordLength), targetHashes.ToArray(), cts.Token), cts.Token);
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
            if (stopwatch.IsRunning)
            {
                cts?.Cancel();
                timerCts?.Cancel();
                stopwatch.Stop();
            }
            foundHashes.Clear();
            currentPassword = "Нет данных";
            currentIndex = 0;
            ResultsTextBox.Clear();
            ThreadProgressListBox.Items.Clear();
            FoundCountTextBlock.Text = $"Найдено: 0/{targetHashes?.Count ?? 0}";
            TimerTextBlock.Text = "Прошедшее время: 00:00:00";
            savedElapsedTicks = 0;
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
            StartButton.IsEnabled = true;
            PauseResumeButton.IsEnabled = false;
            PauseResumeButton.Content = "Пауза";
            ResetButton.IsEnabled = true;
            isPaused = false;
        }

        // Метод взлома для перебора комбинаций
        private void Crack(long start, long end, string[] hashesToCrack, CancellationToken token)
        {
            long charsetSize = charset.Length;

            for (long i = start; i < end; i++)
            {
                token.ThrowIfCancellationRequested();
                currentIndex = i;

                // Генерируем пароль
                StringBuilder password = new StringBuilder();
                long temp = i;
                for (int j = 0; j < passwordLength; j++)
                {
                    password.Append(charset[(int)(temp % charsetSize)]);
                    temp /= charsetSize;
                }
                currentPassword = password.ToString();

                // Вычисляем хэш пароля
                string currentHash = CalculateSha512(currentPassword);
                foreach (string targetHash in hashesToCrack)
                {
                    if (!foundHashes.Contains(targetHash) && currentHash == targetHash)
                    {
                        SaveResult(targetHash, currentPassword);
                        Dispatcher.Invoke(() =>
                        {
                            ResultsTextBox.AppendText($"Пароль найден для {targetHash}: {currentPassword}\n");
                            FoundCountTextBlock.Text = $"Найдено: {foundHashes.Count + 1}/{targetHashes.Count}";
                        });
                        lock (foundHashes)
                        {
                            foundHashes.Add(targetHash);
                        }
                        break;
                    }
                }

                if (foundHashes.Count == hashesToCrack.Length)
                {
                    return;
                }
            }
        }

        // Метод вычисления SHA-512 хэша
        private string CalculateSha512(string input)
        {
            Sha512Digest digest = new Sha512Digest();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] hashBytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBytes, 0);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        // Сохранение прогресса в файл
        private void SaveProgress()
        {
            var progress = new StringBuilder();
            progress.AppendLine($"Elapsed Time: {stopwatch.ElapsedTicks + savedElapsedTicks}");
            progress.AppendLine($"Пароль: {currentPassword}:{currentIndex}");
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
                SaveProgress();
            }
        }

        // Очистка ресурсов после завершения взлома
        private void Cleanup()
        {
            stopwatch.Stop();
            timerCts?.Cancel();
            timerTask?.Wait();
            cts?.Dispose();
            timerCts?.Dispose();
            timerCts = null;
            if (!isPaused)
            {
                StartButton.IsEnabled = true;
                PauseResumeButton.IsEnabled = false;
                PauseResumeButton.Content = "Пауза";
            }
        }
    }
}