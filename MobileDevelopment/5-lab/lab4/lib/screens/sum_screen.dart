import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '/screens/history_cubit/history_cubit.dart';
import '/screens/history_screen.dart';
import '/screens/sum_cubit/sum_cubit.dart';
import '/screens/sum_cubit/sum_state.dart';

// Класс экрана для отображения калькулятора
class SumScreen extends StatelessWidget {
  const SumScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final _formKey = GlobalKey<FormState>(); // Ключ для валидации формы
    final _aController = TextEditingController(); // Контроллер для поля ввода a
    final _bController = TextEditingController(); // Контроллер для поля ввода b

    return Scaffold(
      appBar: AppBar(
        title: const Text('Калькулятор квадратов суммы'), // Заголовок приложения
        leading: IconButton(
          icon: const Icon(Icons.history), // Иконка для перехода к истории
          onPressed: () {
            Navigator.push(
              context,
              MaterialPageRoute(
                builder: (context) => BlocProvider<HistoryCubit>(
                  create: (context) => HistoryCubit(),
                  child: const HistoryScreen(),
                ),
              ),
            );
          },
        ),
      ),
      body: BlocBuilder<SumCubit, SumState>(
        builder: (context, state) {
          if (state is SumUpdateState) {
            return Padding(
              padding: const EdgeInsets.all(16.0),
              child: Form(
                key: _formKey,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    // Поле ввода для числа a
                    TextFormField(
                      controller: _aController,
                      decoration: const InputDecoration(labelText: 'Введите число a'),
                      keyboardType: TextInputType.number,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Пожалуйста, введите число';
                        }
                        if (double.tryParse(value) == null) {
                          return 'Пожалуйста, введите корректное число';
                        }
                        return null;
                      },
                      onChanged: (value) {
                        if (value.isNotEmpty && double.tryParse(value) != null) {
                          context.read<SumCubit>().updateNumbers(
                                double.parse(value),
                                state.b,
                              );
                        }
                      },
                    ),
                    // Поле ввода для числа b
                    TextFormField(
                      controller: _bController,
                      decoration: const InputDecoration(labelText: 'Введите число b'),
                      keyboardType: TextInputType.number,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Пожалуйста, введите число';
                        }
                        if (double.tryParse(value) == null) {
                          return 'Пожалуйста, введите корректное число';
                        }
                        return null;
                      },
                      onChanged: (value) {
                        if (value.isNotEmpty && double.tryParse(value) != null) {
                          context.read<SumCubit>().updateNumbers(
                                state.a,
                                double.parse(value),
                              );
                        }
                      },
                    ),
                    // Чекбокс для согласия на обработку данных
                    CheckboxListTile(
                      title: const Text('Я согласен на обработку данных'),
                      value: state.consentChecked,
                      onChanged: (bool? value) {
                        context.read<SumCubit>().updateConsent(value ?? false);
                      },
                    ),
                    // Кнопка для выполнения расчета
                    ElevatedButton(
                      onPressed: () {
                        if (_formKey.currentState!.validate()) {
                          context.read<SumCubit>().calculate();
                          if (!state.consentChecked) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                content: Text('Пожалуйста, дайте согласие на обработку данных'),
                              ),
                            );
                          }
                        }
                      },
                      child: const Text('Рассчитать'),
                    ),
                    // Отображение результата, если он есть
                    if (state.result != 0.0) ...[
                      const SizedBox(height: 20),
                      Text(
                        'Квадрат суммы: (${state.a} + ${state.b})² = ${state.result}',
                        style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 20),
                      // Кнопка для сброса данных
                      ElevatedButton(
                        onPressed: () {
                          context.read<SumCubit>().reset();
                          _aController.clear();
                          _bController.clear();
                        },
                        child: const Text('Сбросить'),
                      ),
                    ],
                  ],
                ),
              ),
            );
          }
          return Container(); // Пустой контейнер, если состояние не определено
        },
      ),
    );
  }
}