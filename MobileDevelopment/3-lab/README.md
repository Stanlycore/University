# Лабораторная работа №3: Навигация и управление состоянием

Данный проект реализует приложение "Калькулятор квадратов суммы" с использованием `StatefulWidget` для управления состоянием формы и навигацией между экранами в рамках третьего задания по мобильной разработке.

### 1. Создание основного приложения
- **Файл `main.dart`:**
  ```dart
  void main() {
    runApp(const SumOfSquaresCalculator());
  }

  class SumOfSquaresCalculator extends StatelessWidget {
    const SumOfSquaresCalculator({super.key});

    @override
    Widget build(BuildContext context) {
      return MaterialApp(
        title: 'Калькулятор квадратов суммы',
        theme: ThemeData(primarySwatch: Colors.blue),
        home: const FirstScreen(),
      );
    }
  }
  ```

### 2. Реализация первого экрана с формой
- **Класс `FirstScreen`:** Использован `StatefulWidget` для управления состоянием формы.
- Добавлена форма с двумя полями ввода (`a` и `b`), чекбоксом согласия и кнопкой для расчета.
- Реализована проверка полей ввода.
  ```dart
  TextFormField(
    controller: _aController,
    decoration: const InputDecoration(labelText: 'Введите число a'),
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
  )
  ```
- Чекбокс управляется через `setState`:
  ```dart
  CheckboxListTile(
    title: const Text('Я согласен на обработку данных'),
    value: _consentChecked,
    onChanged: (bool? value) {
      setState(() {
        _consentChecked = value ?? false;
      });
    },
  )
  ```

### 3. Навигация на второй экран
- При нажатии на кнопку "Рассчитать" выполняется валидация формы и проверка чекбокса. Если условия выполнены, осуществляется переход на второй экран с передачей параметров `a` и `b`.
  ```dart
  ElevatedButton(
    onPressed: () {
      if (_formKey.currentState!.validate() && _consentChecked) {
        final a = double.parse(_aController.text);
        final b = double.parse(_bController.text);
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => SecondScreen(a: a, b: b),
          ),
        );
      } else if (!_consentChecked) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Пожалуйста, дайте согласие на обработку данных'),
          ),
        );
      }
    },
    child: const Text('Рассчитать'),
  )
  ```

### 4. Реализация второго экрана
- **Класс `SecondScreen`:** Отображает результат расчета `(a + b)²` и предоставляет кнопку для возврата.
  ```dart
  class SecondScreen extends StatelessWidget {
    final double a;
    final double b;

    const SecondScreen({super.key, required this.a, required this.b});

    @override
    Widget build(BuildContext context) {
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
              Text('Число a: $a', style: const TextStyle(fontSize: 20)),
              Text('Число b: $b', style: const TextStyle(fontSize: 20)),
              Text(
                'Квадрат суммы: ($a + $b)² = $result',
                style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: () {
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
  ```