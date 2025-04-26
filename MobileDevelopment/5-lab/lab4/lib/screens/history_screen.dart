import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '/screens/history_cubit/history_cubit.dart';
import '/screens/history_cubit/history_state.dart';

// Класс экрана для отображения истории вычислений
class HistoryScreen extends StatelessWidget {
  const HistoryScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('История вычислений'), // Заголовок экрана
        actions: [
          // Кнопка для очистки истории
          IconButton(
            icon: const Icon(Icons.delete),
            onPressed: () {
              context.read<HistoryCubit>().clearCalculations();
            },
          ),
        ],
      ),
      body: BlocBuilder<HistoryCubit, HistoryState>(
        builder: (context, state) {
          if (state is HistoryLoadedState) {
            final calculations = state.calculations;
            if (calculations.isEmpty) {
              return const Center(child: Text('Нет сохраненных вычислений'));
            }
            return ListView.builder(
              itemCount: calculations.length,
              itemBuilder: (context, index) {
                final calc = calculations[index];
                return ListTile(
                  title: Text('a: ${calc['a']}, b: ${calc['b']}'),
                  subtitle: Text('Результат: ${calc['result']}'),
                );
              },
            );
          }
          return const Center(child: CircularProgressIndicator());
        },
      ),
    );
  }
}