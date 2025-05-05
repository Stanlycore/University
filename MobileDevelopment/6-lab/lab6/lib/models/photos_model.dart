import '/models/camera_model.dart';
import '/models/rover_model.dart';

// Модель для данных о фотографии
class Photos {
  int? id;
  int? sol;
  Camera? camera;
  String? imgSrc;
  String? earthDate;
  Rover? rover;

  Photos({
    this.id,
    this.sol,
    this.camera,
    this.imgSrc,
    this.earthDate,
    this.rover,
  });

  // Фабричный метод для создания экземпляра Photos из JSON
  Photos.fromJson(Map<String, dynamic> json) {
    id = json['id'];
    sol = json['sol'];
    camera = json['camera'] != null ? Camera.fromJson(json['camera']) : null;
    imgSrc = json['img_src'];
    earthDate = json['earth_date'];
    rover = json['rover'] != null ? Rover.fromJson(json['rover']) : null;
  }

  // Метод для преобразования экземпляра Photos в JSON
  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = Map<String, dynamic>();
    data['id'] = this.id;
    data['sol'] = this.sol;
    if (this.camera != null) {
      data['camera'] = this.camera!.toJson();
    }
    data['img_src'] = this.imgSrc;
    data['earth_date'] = this.earthDate;
    if (this.rover != null) {
      data['rover'] = this.rover!.toJson();
    }
    return data;
  }
}