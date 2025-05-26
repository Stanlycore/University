import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/weather_model.dart';

class WeatherService {
  final String apiKey = '1ea984ef39ba45f3860142333252605'; // Replace with your WeatherAPI.com key
  final String baseUrl = 'https://api.weatherapi.com/v1/current.json';

  Future<WeatherModel> fetchWeather(String city) async {
    final response = await http.get(Uri.parse('$baseUrl?key=$apiKey&q=$city&aqi=yes'));

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return WeatherModel(
        city: data['location']['name'],
        temperature: data['current']['temp_c'].toDouble(),
        uvIndex: data['current']['uv'].toDouble(),
        description: data['current']['condition']['text'],
        timestamp: DateTime.now(),
      );
    } else {
      throw Exception('Failed to load weather data');
    }
  }
}