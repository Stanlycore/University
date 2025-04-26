import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '/screens/sum_cubit/sum_state.dart';

// Класс Cubit для управления состоянием приложения
class SumCubit extends Cubit<SumState> {
  SumCubit() : super(SumUpdateState()); // Инициализация с начальным состоянием

  double a = 0.0; // Первое число
  double b = 0.0; // Второе число
  bool consentChecked = false; // Состояние чекбокса согласия

  // Метод для обновления чисел a и b
  void updateNumbers(double newA, double newB) {
    a = newA;
    b = newB;
    emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
  }

  // Метод для обновления состояния чекбокса согласия
  void updateConsent(bool newConsent) {
    consentChecked = newConsent;
    emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
  }

  // Метод для вычисления квадрата суммы чисел
  void calculate() {
    if (consentChecked) {
      final sum = a + b;
      final result = sum * sum;
      emit(SumUpdateState(a: a, b: b, result: result, consentChecked: consentChecked));
      _saveCalculation(a, b, result); // Сохранение результата после вычисления
    }
  }

  // Метод для сброса всех данных к начальному состоянию
  void reset() {
    a = 0.0;
    b = 0.0;
    consentChecked = false;
    emit(SumUpdateState(a: a, b: b, result: 0.0, consentChecked: consentChecked));
  }

  // Приватный метод для сохранения вычисления в SharedPreferences
  Future<void> _saveCalculation(double a, double b, double result) async {
    final prefs = await SharedPreferences.getInstance();
    // Получаем текущий список вычислений или создаем новый
    List<String> calculations = prefs.getStringList('calculations') ?? [];
    // Добавляем новое вычисление в формате "a,b,result"
    calculations.add('$a,$b,$result');
    // Сохраняем обновленный список
    await prefs.setStringList('calculations', calculations);
  }
}