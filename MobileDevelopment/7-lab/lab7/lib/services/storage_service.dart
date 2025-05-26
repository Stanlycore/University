import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/weather_model.dart';

class StorageService {
  static const String weatherHistoryKey = 'weather_history';
  static const String calcHistoryKey = 'calc_history';
  static const String photosHistoryKey = 'photos_history';

  Future<void> saveWeather(WeatherModel weather) async {
    final prefs = await SharedPreferences.getInstance();
    final history = await getWeatherHistory();
    history.add(weather);
    final encoded = history.map((w) => jsonEncode(w.toJson())).toList();
    await prefs.setStringList(weatherHistoryKey, encoded);
  }

  Future<List<WeatherModel>> getWeatherHistory() async {
    final prefs = await SharedPreferences.getInstance();
    final encoded = prefs.getStringList(weatherHistoryKey) ?? [];
    return encoded.map((e) => WeatherModel.fromJson(jsonDecode(e))).toList();
  }

  Future<void> clearWeatherHistory() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(weatherHistoryKey);
  }

  Future<void> saveCalculation(String entry) async {
    final prefs = await SharedPreferences.getInstance();
    final history = await getCalculationHistory();
    history.add(entry);
    await prefs.setStringList(calcHistoryKey, history);
  }

  Future<List<String>> getCalculationHistory() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getStringList(calcHistoryKey) ?? [];
  }

  Future<void> clearCalculationHistory() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(calcHistoryKey);
  }

  Future<void> savePhotoInfo(String path, String weatherType) async {
    final prefs = await SharedPreferences.getInstance();
    final history = await getPhotosHistory();
    history.add('$path|$weatherType|${DateTime.now()}');
    await prefs.setStringList(photosHistoryKey, history);
  }

  Future<List<String>> getPhotosHistory() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getStringList(photosHistoryKey) ?? [];
  }
}