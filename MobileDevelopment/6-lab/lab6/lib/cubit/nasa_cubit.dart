import '/cubit/nasa_state.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '/models/nasa_model.dart';
import '/requests/nasa_api.dart';

// Кубит для управления состоянием загрузки данных NASA
class NasaCubit extends Cubit<NasaState> {
  NasaCubit() : super(NasaLoadingState()); // Начальное состояние - загрузка

  // Метод для асинхронной загрузки данных NASA
  Future<void> loadData() async {
    try {
      // Получение данных из API NASA
      Map<String, dynamic> apiData = await getNasaData();
      // Преобразование JSON в модель Nasa
      Nasa nasaData = Nasa.fromJson(apiData);
      // Переход в состояние успешной загрузки с полученными данными
      emit(NasaLoadedState(data: nasaData));
    } catch (e) {
      // Переход в состояние ошибки, если что-то пошло не так
      emit(NasaErrorState());
    }
  }
}