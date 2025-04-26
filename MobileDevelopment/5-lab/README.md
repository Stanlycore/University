# Лабораторная работа №5: Хранение данных

Данный проект расширяет приложение "Калькулятор квадратов суммы" из Лабораторной работы №4, добавляя функциональность сохранения и отображения истории вычислений с использованием `SharedPreferences`


### 1. Добавление зависимости `shared_preferences`
- В файл `pubspec.yaml` добавлена зависимость `shared_preferences`:
  ```yaml
  dependencies:
    flutter:
      sdk: flutter
    flutter_bloc: ^7.2.0
    shared_preferences: ^2.0.15
  ```

### 2. Обновление структуры проекта
- Добавлены новые файлы в папку `screens`:
  - Подпапка `history_cubit` с файлами `history_cubit.dart` и `history_state.dart`.
  - Файл `history_screen.dart`.

### 3. Реализация сохранения данных в `SumCubit`
- **Файл `sum_cubit.dart`:** Добавлен метод `_saveCalculation` для сохранения вычислений в `SharedPreferences`.
  ```dart
  Future<void> _saveCalculation(double a, double b, double result) async {
    final prefs = await SharedPreferences.getInstance();
    List<String> calculations = prefs.getStringList('calculations') ?? [];
    calculations.add('$a,$b,$result');
    await prefs.setStringList('calculations', calculations);
  }
  ```
- Метод вызывается в `calculate` после успешного вычисления:
  ```dart
  void calculate() {
    if (consentChecked) {
      final sum = a + b;
      final result = sum * sum;
      emit(SumUpdateState(a: a, b: b, result: result, consentChecked: consentChecked));
      _saveCalculation(a, b, result);
    }
  }
  ```

### 4. Создание Cubit и состояния для экрана истории
- **Файл `history_state.dart`:** Определено состояние для хранения списка вычислений.
  ```dart
  abstract class HistoryState {}

  class HistoryLoadedState extends HistoryState {
    final List<Map<String, double>> calculations;

    HistoryLoadedState(this.calculations);
  }
  ```
- **Файл `history_cubit.dart`:** Реализован Cubit для загрузки и очистки данных.
  ```dart
  class HistoryCubit extends Cubit<HistoryState> {
    HistoryCubit() : super(HistoryLoadedState([])) {
      loadCalculations();
    }

    Future<void> loadCalculations() async {
      final prefs = await SharedPreferences.getInstance();
      final List<String> calculations = prefs.getStringList('calculations') ?? [];
      List<Map<String, double>> parsedCalculations = calculations.map((calc) {
        final parts = calc.split(',');
        return {
          'a': double.parse(parts[0]),
          'b': double.parse(parts[1]),
          'result': double.parse(parts[2]),
        };
      }).toList();
      emit(HistoryLoadedState(parsedCalculations));
    }

    Future<void> clearCalculations() async {
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove('calculations');
      emit(HistoryLoadedState([]));
    }
  }
  ```

### 5. Создание экрана истории
- **Файл `history_screen.dart`:** Реализован экран для отображения списка сохраненных вычислений.
  ```dart
  class HistoryScreen extends StatelessWidget {
    const HistoryScreen({super.key});

    @override
    Widget build(BuildContext context) {
      return Scaffold(
        appBar: AppBar(
          title: const Text('История вычислений'),
          actions: [
            IconButton(
              icon: const Icon(Icons.delete),
              onPressed: () {
                context.read<HistoryCubit>().clearCalculations();
              },
            ),
          ],
        ),
        body: BlocBuilder<HistoryCubit, HistoryState>(
          builder: (context, state) {
            if (state is HistoryLoadedState) {
              final calculations = state.calculations;
              if (calculations.isEmpty) {
                return const Center(child: Text('Нет сохраненных вычислений'));
              }
              return ListView.builder(
                itemCount: calculations.length,
                itemBuilder: (context, index) {
                  final calc = calculations[index];
                  return ListTile(
                    title: Text('a: ${calc['a']}, b: ${calc['b']}'),
                    subtitle: Text('Результат: ${calc['result']}'),
                  );
                },
              );
            }
            return const Center(child: CircularProgressIndicator());
          },
        ),
      );
    }
  }
  ```

### 6. Добавление навигации на экран истории
- **Файл `sum_screen.dart`:** В `AppBar` добавлена кнопка для перехода на экран истории.
  ```dart
  appBar: AppBar(
    title: const Text('Калькулятор квадратов суммы'),
    leading: IconButton(
      icon: const Icon(Icons.history),
      onPressed: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => BlocProvider<HistoryCubit>(
              create: (context) => HistoryCubit(),
              child: const HistoryScreen(),
            ),
          ),
        );
      },
    ),
  ),
  ```

### 7. Обеспечение доступа к Cubit
- **Файл `sum_provider.dart`:** Обновлен для предоставления только `SumCubit`, так как `HistoryCubit` предоставляется непосредственно при навигации.
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