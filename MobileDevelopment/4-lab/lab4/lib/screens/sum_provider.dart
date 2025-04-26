import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '/screens/sum_cubit/sum_cubit.dart';
import '/screens/sum_screen.dart';

// Класс провайдера для предоставления Cubit экрану
class SumProvider extends StatelessWidget {
  const SumProvider({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocProvider<SumCubit>(
      create: (context) => SumCubit(), // Создание экземпляра Cubit
      child: const SumScreen(), // Передача экрана как дочернего виджета
    );
  }
}