import '/models/nasa_model.dart';

// Абстрактное базовое состояние для кубита NASA
abstract class NasaState {}

// Состояние во время загрузки данных
class NasaLoadingState extends NasaState {}

// Состояние при успешной загрузке данных
class NasaLoadedState extends NasaState {
  final Nasa data;
  NasaLoadedState({required this.data});
}

// Состояние при возникновении ошибки во время загрузки
class NasaErrorState extends NasaState {}