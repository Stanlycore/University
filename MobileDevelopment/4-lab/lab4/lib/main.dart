import 'package:flutter/material.dart';
import '/screens/sum_provider.dart';

// Точка входа приложения
void main() {
  runApp(const SumOfSquaresCalculator()); // Запуск приложения
}

// Главный класс приложения
class SumOfSquaresCalculator extends StatelessWidget {
  const SumOfSquaresCalculator({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Калькулятор квадратов суммы - Непомнящих Станислав', // Заголовок приложения
      theme: ThemeData(primarySwatch: Colors.blue), // Тема приложения
      home: const SumProvider(), // Установка провайдера как домашнего экрана
    );
  }
}