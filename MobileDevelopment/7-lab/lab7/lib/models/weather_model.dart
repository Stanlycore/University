class WeatherModel {
  final String city;
  final double temperature;
  final double uvIndex;
  final String description;
  final DateTime timestamp;

  WeatherModel({
    required this.city,
    required this.temperature,
    required this.uvIndex,
    required this.description,
    required this.timestamp,
  });

  Map<String, dynamic> toJson() => {
        'city': city,
        'temperature': temperature,
        'uvIndex': uvIndex,
        'description': description,
        'timestamp': timestamp.toIso8601String(),
      };

  factory WeatherModel.fromJson(Map<String, dynamic> json) => WeatherModel(
        city: json['city'],
        temperature: json['temperature'],
        uvIndex: json['uvIndex'],
        description: json['description'],
        timestamp: DateTime.parse(json['timestamp']),
      );
}