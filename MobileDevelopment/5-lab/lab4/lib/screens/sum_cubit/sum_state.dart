// Абстрактный класс для базового состояния Cubit
abstract class SumState {}

// Класс состояния для обновления данных в Cubit
class SumUpdateState extends SumState {
  final double a; // Первое число
  final double b; // Второе число
  final double result; // Результат вычисления
  final bool consentChecked; // Состояние чекбокса согласия

  SumUpdateState({
    this.a = 0.0,
    this.b = 0.0,
    this.result = 0.0,
    this.consentChecked = false,
  });
}