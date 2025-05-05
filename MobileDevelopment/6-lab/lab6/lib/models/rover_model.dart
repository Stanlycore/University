// Модель для данных о марсоходе
class Rover {
  int? id;
  String? name;
  String? landingDate;
  String? launchDate;
  String? status;

  Rover({this.id, this.name, this.landingDate, this.launchDate, this.status});

  // Фабричный метод для создания экземпляра Rover из JSON
  Rover.fromJson(Map<String, dynamic> json) {
    id = json['id'];
    name = json['name'];
    landingDate = json['landing_date'];
    launchDate = json['launch_date'];
    status = json['status'];
  }

  // Метод для преобразования экземпляра Rover в JSON
  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = Map<String, dynamic>();
    data['id'] = this.id;
    data['name'] = this.name;
    data['landing_date'] = this.landingDate;
    data['launch_date'] = this.launchDate;
    data['status'] = this.status;
    return data;
  }
}