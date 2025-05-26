import 'dart:io';
import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:path_provider/path_provider.dart';
import 'package:path/path.dart' as path;
import 'package:shared_preferences/shared_preferences.dart';

class CameraScreen extends StatefulWidget {
  final List<CameraDescription> cameras;

  CameraScreen({required this.cameras});

  @override
  _CameraScreenState createState() => _CameraScreenState();
}

class _CameraScreenState extends State<CameraScreen> {
  late CameraController _controller;
  late Future<void> _initializeControllerFuture;
  String? _imagePath;
  bool _isTakingPicture = false;
  String? _selectedWeatherType;
  final List<String> _weatherTypes = [
    'Солнечно',
    'Облачно',
    'Дождь',
    'Снег',
    'Туман',
    'Гроза'
  ];

  @override
  void initState() {
    super.initState();
    _controller = CameraController(widget.cameras[0], ResolutionPreset.high);
    _initializeControllerFuture = _controller.initialize().then((_) {
      if (!mounted) return;
      setState(() {});
    });
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<String> _getPermanentPhotoPath() async {
    final directory = await getApplicationDocumentsDirectory();
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return path.join(directory.path, 'weather_photo_$timestamp.jpg');
  }

  Future<void> _takePicture() async {
    if (_isTakingPicture || _selectedWeatherType == null) return;
    
    setState(() {
      _isTakingPicture = true;
    });

    try {
      await _initializeControllerFuture;
      final tempImage = await _controller.takePicture();
      
      // Переносим фото в постоянное хранилище
      final permanentPath = await _getPermanentPhotoPath();
      await File(tempImage.path).copy(permanentPath);
      
      // Сохраняем информацию о фото
      final prefs = await SharedPreferences.getInstance();
      final history = prefs.getStringList('photos_history') ?? [];
      history.add('$permanentPath|$_selectedWeatherType|${DateTime.now()}');
      await prefs.setStringList('photos_history', history);

      if (mounted) {
        setState(() {
          _imagePath = permanentPath;
          _isTakingPicture = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isTakingPicture = false;
        });
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ошибка: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Сделать фото погоды')),
      body: Column(
        children: [
          Expanded(
            child: Container(
              color: Colors.black,
              child: Center(
                child: FutureBuilder<void>(
                  future: _initializeControllerFuture,
                  builder: (context, snapshot) {
                    if (snapshot.connectionState == ConnectionState.done) {
                      return CameraPreview(_controller);
                    } else {
                      return CircularProgressIndicator();
                    }
                  },
                ),
              ),
            ),
          ),
          Padding(
            padding: EdgeInsets.all(16.0),
            child: Column(
              children: [
                DropdownButtonFormField<String>(
                  value: _selectedWeatherType,
                  hint: Text('Выберите тип погоды'),
                  items: _weatherTypes.map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(value),
                    );
                  }).toList(),
                  onChanged: (newValue) {
                    setState(() {
                      _selectedWeatherType = newValue;
                    });
                  },
                ),
                SizedBox(height: 16),
                _isTakingPicture
                    ? CircularProgressIndicator()
                    : ElevatedButton(
                        onPressed: _selectedWeatherType != null ? _takePicture : null,
                        child: Text('Сделать фото'),
                      ),
              ],
            ),
          ),
          if (_imagePath != null) ...[
            Container(
              padding: EdgeInsets.all(8.0),
              color: Colors.grey[200],
              child: Column(
                children: [
                  Text('Сохранено: $_imagePath'),
                  SizedBox(height: 8),
                  Container(
                    height: 200,
                    width: MediaQuery.of(context).size.width,
                    child: Image.file(
                      File(_imagePath!),
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) {
                        return Center(child: Text('Не удалось загрузить изображение'));
                      },
                    ),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }
}