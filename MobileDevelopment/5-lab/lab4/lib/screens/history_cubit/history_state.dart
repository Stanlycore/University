// Абстрактный класс для базового состояния Cubit истории
abstract class HistoryState {}

// Класс состояния для отображения списка вычислений
class HistoryLoadedState extends HistoryState {
  final List<Map<String, double>> calculations; // Список вычислений

  HistoryLoadedState(this.calculations);
}