import 'package:http/http.dart' as http;
import 'dart:convert';

// Функция для получения данных NASA для марсохода Curiosity, Sol 100
Future<Map<String, dynamic>> getNasaData() async {
  // Формирование URI для запроса к API NASA
  Uri url = Uri.parse(
    'https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol=100&api_key=eQnprvXukgfNomTanZiHT1DqLApcABzFjI350dyZ',
  );
  // Выполнение HTTP GET запроса
  final response = await http.get(url);

  // Проверка успешности ответа (код 200)
  if (response.statusCode == 200) {
    return json.decode(response.body); // Декодирование и возврат JSON данных
  } else {
    // Выброс исключения в случае неудачного запроса
    throw Exception('Ошибка: ${response.reasonPhrase}');
  }
}