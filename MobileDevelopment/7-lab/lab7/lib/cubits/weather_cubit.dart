import 'package:flutter_bloc/flutter_bloc.dart';
import '../models/weather_model.dart';
import '../services/storage_service.dart';
import '../services/weather_service.dart';

class WeatherCubit extends Cubit<WeatherState> {
  final WeatherService _weatherService = WeatherService();
  final StorageService _storageService = StorageService();

  WeatherCubit() : super(WeatherInitial());

  Future<void> fetchWeather(String city) async {
    emit(WeatherLoading());
    try {
      final weather = await _weatherService.fetchWeather(city);
      await _storageService.saveWeather(weather);
      final history = await _storageService.getWeatherHistory();
      emit(WeatherLoaded(weather, history));
    } catch (e) {
      emit(WeatherError('Failed to fetch weather: $e'));
    }
  }

  Future<void> loadHistory() async {
    emit(WeatherLoading());
    try {
      final history = await _storageService.getWeatherHistory();
      emit(WeatherLoaded(null, history));
    } catch (e) {
      emit(WeatherError('Failed to load history: $e'));
    }
  }
}

abstract class WeatherState {}

class WeatherInitial extends WeatherState {}

class WeatherLoading extends WeatherState {}

class WeatherLoaded extends WeatherState {
  final WeatherModel? currentWeather;
  final List<WeatherModel> history;

  WeatherLoaded(this.currentWeather, this.history);
}

class WeatherError extends WeatherState {
  final String message;

  WeatherError(this.message);
}