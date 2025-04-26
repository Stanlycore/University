# Лабораторная работа №4: Управление состоянием

Данный проект демонстрирует реализацию управления состоянием в приложении "Калькулятор квадратов суммы" с использованием BLoC-паттерна и Cubit

### 1. Инициализация проекта с зависимостью `flutter_bloc`
- Добавлена зависимость `flutter_bloc` в файл `pubspec.yaml` для использования Cubit:
  ```yaml
  dependencies:
    flutter:
      sdk: flutter
    flutter_bloc: ^7.2.0
  ```

### 2. Создание структуры проекта
- Создана папка `screens`, содержащая:
  - Подпапку `sum_cubit` с файлами `sum_cubit.dart` и `sum_state.dart`.
  - Файлы `sum_screen.dart` и `sum_provider.dart`.

### 3. Реализация Cubit и состояния
- **Файл `sum_state.dart`:** Определены состояния для Cubit.
  ```dart
  abstract class SumState {}

  class SumUpdateState extends SumState {
    final double a;
    final double b;
    final double result;
    final bool consentChecked;

    SumUpdateState({
      this.a = 0.0,
      this.b = 0.0,
      this.result = 0.0,
      this.consentChecked = false,
    });
  }
  ```
- **Файл `sum_cubit.dart`:** Реализован Cubit для управления состоянием и выполнения вычислений.
  ```dart
  class SumCubit extends Cubit<SumState> {
    SumCubit() : super(SumUpdateState());

    double a = 0.0;
    double b = 0.0;
    bool consentChecked = false;

    void updateNumbers(double newA, double newB) {
      a = newA;
      b = newB;
      emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
    }

    void updateConsent(bool newConsent) {
      consentChecked = newConsent;
      emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
    }

    void calculate() {
      if (consentChecked) {
        final sum = a + b;
        final result = sum * sum;
        emit(SumUpdateState(a: a, b: b, result: result, consentChecked: consentChecked));
      }
    }

    void reset() {
      a = 0.0;
      b = 0.0;
      consentChecked = false;
      emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
    }
  }
  ```

### 4. Реализация основного экрана
- **Файл `sum_screen.dart`:** Создан экран с использованием `BlocBuilder` для отображения формы ввода, чекбокса согласия, кнопки расчета и результата.
  ```dart
  return Scaffold(
    appBar: AppBar(
      title: const Text('Калькулятор квадратов суммы'),
    ),
    body: BlocBuilder<SumCubit, SumState>(
      builder: (context, state) {
        if (state is SumUpdateState) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
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
                    onChanged: (value) {
                      if (value.isNotEmpty && double.tryParse(value) != null) {
                        context.read<SumCubit>().updateNumbers(
                              double.parse(value),
                              state.b,
                            );
                      }
                    },
                  ),
                  // Аналогично для числа b
                  CheckboxListTile(
                    title: const Text('Я согласен на обработку данных'),
                    value: state.consentChecked,
                    onChanged: (bool? value) {
                      context.read<SumCubit>().updateConsent(value ?? false);
                    },
                  ),
                  ElevatedButton(
                    onPressed: () {
                      if (_formKey.currentState!.validate()) {
                        context.read<SumCubit>().calculate();
                        if (!state.consentChecked) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Пожалуйста, дайте согласие на обработку данных'),
                            ),
                          );
                        }
                      }
                    },
                    child: const Text('Рассчитать'),
                  ),
                  if (state.result != 0.0) ...[
                    const SizedBox(height: 20),
                    Text(
                      'Квадрат суммы: (${state.a} + ${state.b})² = ${state.result}',
                      style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(height: 20),
                    ElevatedButton(
                      onPressed: () {
                        context.read<SumCubit>().reset();
                        _aController.clear();
                        _bController.clear();
                      },
                      child: const Text('Сбросить'),
                    ),
                  ],
                ],
              ),
            ),
          );
        }
        return Container();
      },
    ),
  );
  ```

### 5. Создание провайдера
- **Файл `sum_provider.dart`:** Реализован провайдер для предоставления `SumCubit` экрану.
  ```dart
  class SumProvider extends StatelessWidget {
    const SumProvider({super.key});

    @override
    Widget build(BuildContext context) {
      return BlocProvider<SumCubit>(
        create: (context) => SumCubit(),
        child: const SumScreen(),
      );
    }
  }
  ```

### 6. Обновление точки входа
- **Файл `main.dart`:** Обновлен для использования `SumProvider` как начального экрана.
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
        home: const SumProvider(),
      );
    }
  }
  ```