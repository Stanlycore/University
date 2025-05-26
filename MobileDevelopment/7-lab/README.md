# Лабораторная работа №7: Мобильное приложение с API и локальным хранилищем

Данный проект представляет собой погодное приложение с функционалом работы с API, локальным хранилищем, расчетами и дополнительными возможностями (работа с камерой), реализованное с использованием BLoC-паттерна (Cubit) в рамках седьмого задания по мобильной разработке.

## Реализованный функционал

### 1. Работа с WeatherAPI.com
- Настроен HTTP-запрос к WeatherAPI для получения текущей погоды:
  ```dart
  Future<WeatherModel> fetchWeather(String city) async {
    final response = await http.get(Uri.parse('$baseUrl?key=$apiKey&q=$city&aqi=yes'));
    
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return WeatherModel(
        city: data['location']['name'],
        temperature: data['current']['temp_c'].toDouble(),
        uvIndex: data['current']['uv'].toDouble(),
        description: data['current']['condition']['text'],
        timestamp: DateTime.now(),
      );
    }
  }
  ```

### 2. Локальное хранилище истории запросов
- Реализовано сохранение истории запросов погоды с использованием SharedPreferences:
  ```dart
  Future<void> saveWeather(WeatherModel weather) async {
    final prefs = await SharedPreferences.getInstance();
    final history = await getWeatherHistory();
    history.add(weather);
    final encoded = history.map((w) => jsonEncode(w.toJson())).toList();
    await prefs.setStringList(weatherHistoryKey, encoded);
  }
  ```

### 3. Экран информации о разработчике
- Реализован отдельный экран с персональной информацией:
  ```dart
  class DeveloperScreen extends StatelessWidget {
    @override
    Widget build(BuildContext context) {
      return Scaffold(
        appBar: AppBar(title: Text('О разработчике')),
        body: Padding(
          padding: EdgeInsets.all(16.0),
          child: Column(
            children: [
              Text('Имя: Непомнящих Станислав', style: TextStyle(fontSize: 20)),
              Text('Группа: ИВТ-22', style: TextStyle(fontSize: 16)),
              Text('Email: cnfybckfd24@mail.ru', style: TextStyle(fontSize: 16)),
            ],
          ),
        ),
      );
    }
  }
  ```

### 4. Экран расчетов с историей
- Реализована оценка уровня опасности УФ-излучения:
  ```dart
  String _calculateUvDanger(double uvIndex) {
    if (uvIndex <= 2) return 'Низкая';
    if (uvIndex <= 5) return 'Умеренная';
    if (uvIndex <= 7) return 'Высокая';
    if (uvIndex <= 10) return 'Очень высокая';
    return 'Экстремальная';
  }
  ```

### 5. Дополнительный функционал (работа с камерой)
- Интеграция пакета camera для съемки фото:
  ```dart
  _controller = CameraController(widget.cameras[0], ResolutionPreset.high);
  _initializeControllerFuture = _controller.initialize();
  ```
- Сохранение фото в постоянное хранилище с метаданными:
  ```dart
  final permanentPath = await _getPermanentPhotoPath();
  await File(tempImage.path).copy(permanentPath);
  await _storageService.savePhotoInfo(permanentPath, _selectedWeatherType!);
  ```
- Добавлен выбор типа погоды при съемке