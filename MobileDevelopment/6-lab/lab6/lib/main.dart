import 'package:flutter/material.dart';
import '/screens/photos_screen.dart';

// Точка входа в приложение
void main() {
  runApp(const MainApp());
}

// Главный виджет приложения
class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: "Фотографии марсохода Curiosity",
      theme: ThemeData(
        primarySwatch: Colors.blue,
        scaffoldBackgroundColor: Colors.white,
      ),
      home: const PhotosScreen(),
    );
  }
}