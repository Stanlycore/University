import 'package:flutter/material.dart';

void main() {
  // Запускаем приложение Flutter
  runApp(const SumOfSquaresCalculator());
}

// Главный класс приложения, наследующий StatelessWidget
class SumOfSquaresCalculator extends StatelessWidget {
  const SumOfSquaresCalculator({super.key});

  @override
  Widget build(BuildContext context) {
    // MaterialApp создает базовую структуру приложения и автоматически предоставляет экземпляр Navigator
    return MaterialApp(
      title: 'Калькулятор квадратов суммы',
      theme: ThemeData(primarySwatch: Colors.blue),
      home: const FirstScreen(),
    );
  }
}

// Первый экран приложения, наследующий StatefulWidget для управления состоянием формы
class FirstScreen extends StatefulWidget {
  const FirstScreen({super.key});

  @override
  FirstScreenState createState() => FirstScreenState();
}

// Состояние первого экрана
class FirstScreenState extends State<FirstScreen> {
  // Ключ формы для валидации
  final _formKey = GlobalKey<FormState>();
  // Контроллеры для полей ввода a и b
  final _aController = TextEditingController();
  final _bController = TextEditingController();
  // Переменная для состояния чекбокса согласия
  bool _consentChecked = false;

  @override
  void dispose() {
    // Освобождаем ресурсы контроллеров при уничтожении виджета
    _aController.dispose();
    _bController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Калькулятор квадратов суммы - Непомнящих Станислав'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          // Виджет Form объединяет поля ввода и управляет их валидацией
          key: _formKey,
          child: Column(
            children: [
              // Поле ввода для числа a
              TextFormField(
                controller: _aController,
                decoration: const InputDecoration(
                  labelText: 'Введите число a',
                ),
                // Указываем числовую клавиатуру
                keyboardType: TextInputType.number,
                // Валидация поля
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Пожалуйста, введите число';
                  }
                  if (double.tryParse(value) == null) {
                    return 'Пожалуйста, введите корректное число';
                  }
                  return null;
                },
              ),
              // Поле ввода для числа b
              TextFormField(
                controller: _bController,
                decoration: const InputDecoration(
                  labelText: 'Введите число b',
                ),
                keyboardType: TextInputType.number,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Пожалуйста, введите число';
                  }
                  if (double.tryParse(value) == null) {
                    return 'Пожалуйста, введите корректное число';
                  }
                  return null;
                },
              ),
              // Чекбокс для согласия на обработку данных
              CheckboxListTile(
                title: const Text('Я согласен на обработку данных'),
                value: _consentChecked,
                onChanged: (bool? value) {
                  setState(() {
                    // Обновляем состояние чекбокса
                    _consentChecked = value ?? false;
                  });
                },
              ),
              // Кнопка для выполнения расчета и навигации
              ElevatedButton(
                onPressed: () {
                  // Проверяем валидность формы и состояние чекбокса
                  if (_formKey.currentState!.validate() && _consentChecked) {
                    // Получаем данные из полей
                    final a = double.parse(_aController.text);
                    final b = double.parse(_bController.text);
                    // Переходим на второй экран с передачей параметров
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => SecondScreen(a: a, b: b),
                      ),
                    );
                  } else if (!_consentChecked) {
                    // Показываем сообщение, если чекбокс не отмечен
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Пожалуйста, дайте согласие на обработку данных'),
                      ),
                    );
                  }
                },
                child: const Text('Рассчитать'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// Второй экран для отображения результата
class SecondScreen extends StatelessWidget {
  // Параметры, переданные с первого экрана
  final double a;
  final double b;

  const SecondScreen({super.key, required this.a, required this.b});

  @override
  Widget build(BuildContext context) {
    // Выполняем расчет: (a + b) ^ 2
    final sum = a + b;
    final result = sum * sum;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Результат расчета'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // Отображаем введенные числа
            Text(
              'Число a: $a',
              style: const TextStyle(fontSize: 20),
            ),
            Text(
              'Число b: $b',
              style: const TextStyle(fontSize: 20),
            ),
            // Отображаем результат расчета
            Text(
              'Квадрат суммы: ($a + $b)² = $result',
              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 20),
            // Кнопка для возврата на первый экран
            ElevatedButton(
              onPressed: () {
                // Закрываем текущий экран
                Navigator.pop(context);
              },
              child: const Text('Назад'),
            ),
          ],
        ),
      ),
    );
  }
}