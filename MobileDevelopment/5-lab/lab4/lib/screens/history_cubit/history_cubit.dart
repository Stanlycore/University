import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '/screens/history_cubit/history_state.dart';

// Класс Cubit для управления состоянием экрана истории
class HistoryCubit extends Cubit<HistoryState> {
  HistoryCubit() : super(HistoryLoadedState([])) {
    loadCalculations(); // Загрузка вычислений при инициализации
  }

  // Метод для загрузки вычислений из SharedPreferences
  Future<void> loadCalculations() async {
    final prefs = await SharedPreferences.getInstance();
    final List<String> calculations = prefs.getStringList('calculations') ?? [];
    // Преобразуем строки в список словарей
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

  // Метод для очистки всех сохраненных вычислений
  Future<void> clearCalculations() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('calculations');
    emit(HistoryLoadedState([])); // Обновляем состояние с пустым списком
  }
}