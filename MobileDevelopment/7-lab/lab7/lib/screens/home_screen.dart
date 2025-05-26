import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../cubits/weather_cubit.dart';
import '../models/weather_model.dart';
import '../services/storage_service.dart';
import 'developer_screen.dart';
import 'calculations_screen.dart';
import 'camera_screen.dart';

class HomeScreen extends StatelessWidget {
  final List<CameraDescription> cameras;
  final TextEditingController _controller = TextEditingController();
  final List<String> _citySuggestions = [
    'Moscow', 'London', 'Paris', 'Berlin', 'Tokyo',
    'New York', 'Beijing', 'Sydney', 'Dubai', 'Istanbul'
  ];

  HomeScreen({required this.cameras});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Погодное приложение'),
        actions: [
          IconButton(
            icon: Icon(Icons.person),
            onPressed: () => Navigator.push(
              context,
              MaterialPageRoute(builder: (context) => DeveloperScreen()),
            ),
          ),
          IconButton(
            icon: Icon(Icons.calculate),
            onPressed: () {
              final state = context.read<WeatherCubit>().state;
              WeatherModel? currentWeather;
              if (state is WeatherLoaded && state.currentWeather != null) {
                currentWeather = state.currentWeather;
              }
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => CalculationsScreen(initialWeather: currentWeather),
                ),
              );
            },
          ),
          IconButton(
            icon: Icon(Icons.camera_alt),
            onPressed: () => Navigator.push(
              context,
              MaterialPageRoute(builder: (context) => CameraScreen(cameras: cameras)),
            ),
          ),
        ],
      ),
      body: BlocConsumer<WeatherCubit, WeatherState>(
        listener: (context, state) {
          if (state is WeatherError) {
            ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(state.message)));
          }
        },
        builder: (context, state) {
          return Padding(
            padding: EdgeInsets.all(16.0),
            child: Column(
              children: [
                Autocomplete<String>(
                  optionsBuilder: (TextEditingValue textEditingValue) {
                    if (textEditingValue.text.isEmpty) {
                      return const Iterable<String>.empty();
                    }
                    return _citySuggestions.where((String option) {
                      return option.toLowerCase().contains(textEditingValue.text.toLowerCase());
                    });
                  },
                  onSelected: (String selection) {
                    _controller.text = selection;
                    context.read<WeatherCubit>().fetchWeather(selection);
                  },
                  fieldViewBuilder: (
                    BuildContext context,
                    TextEditingController controller,
                    FocusNode focusNode,
                    VoidCallback onFieldSubmitted,
                  ) {
                    return TextField(
                      controller: controller,
                      focusNode: focusNode,
                      decoration: InputDecoration(
                        labelText: 'Введите город (на английском)',
                        suffixIcon: IconButton(
                          icon: Icon(Icons.search),
                          onPressed: () {
                            if (controller.text.isNotEmpty) {
                              context.read<WeatherCubit>().fetchWeather(controller.text);
                            }
                          },
                        ),
                      ),
                    );
                  },
                ),
                SizedBox(height: 16),
                if (state is WeatherLoading)
                  CircularProgressIndicator()
                else if (state is WeatherLoaded && state.currentWeather != null)
                  Column(
                    children: [
                      Text(
                        'Погода в ${state.currentWeather!.city}',
                        style: TextStyle(fontSize: 24),
                      ),
                      Text('Температура: ${state.currentWeather!.temperature}°C'),
                      Text('УФ-индекс: ${state.currentWeather!.uvIndex}'),
                      Text('Описание: ${state.currentWeather!.description}'),
                    ],
                  ),
                SizedBox(height: 16),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('История запросов', style: TextStyle(fontSize: 20)),
                    if (state is WeatherLoaded && state.history.isNotEmpty)
                      TextButton(
                        onPressed: () async {
                          await StorageService().clearWeatherHistory();
                          context.read<WeatherCubit>().loadHistory();
                        },
                        child: Text('Очистить'),
                      ),
                  ],
                ),
                Expanded(
                  child: ListView.builder(
                    itemCount: state is WeatherLoaded ? state.history.length : 0,
                    itemBuilder: (context, index) {
                      final weather = (state as WeatherLoaded).history[index];
                      return ListTile(
                        title: Text('${weather.city} - ${weather.description}'),
                        subtitle: Text('${weather.temperature}°C, УФ: ${weather.uvIndex}, ${weather.timestamp}'),
                      );
                    },
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}