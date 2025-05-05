import '/models/photos_model.dart';

// Модель для данных NASA, содержащая список фотографий
class Nasa {
  List<Photos>? photos;

  Nasa({this.photos});

  // Фабричный метод для создания экземпляра Nasa из JSON
  Nasa.fromJson(Map<String, dynamic> json) {
    if (json['photos'] != null) {
      photos = <Photos>[];
      json['photos'].forEach((v) {
        photos!.add(Photos.fromJson(v));
      });
    }
  }

  // Метод для преобразования экземпляра Nasa в JSON
  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = Map<String, dynamic>();
    if (this.photos != null) {
      data['photos'] = this.photos!.map((v) => v.toJson()).toList();
    }
    return data;
  }
}