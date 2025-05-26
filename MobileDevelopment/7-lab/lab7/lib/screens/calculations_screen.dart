import 'package:flutter/material.dart';
import '../models/weather_model.dart';
import '../services/storage_service.dart';

class CalculationsScreen extends StatefulWidget {
  final WeatherModel? initialWeather;

  CalculationsScreen({this.initialWeather});

  @override
  _CalculationsScreenState createState() => _CalculationsScreenState();
}

class _CalculationsScreenState extends State<CalculationsScreen> {
  double _selectedUvIndex = 0;
  String _uvDangerLevel = '';
  List<String> _history = [];
  final StorageService _storageService = StorageService();

  @override
  void initState() {
    super.initState();
    _loadHistory();
    if (widget.initialWeather != null) {
      _selectedUvIndex = widget.initialWeather!.uvIndex;
      _calculateAndSaveUvDanger(widget.initialWeather!.uvIndex);
    }
  }

  void _calculateAndSaveUvDanger(double uvIndex) {
    final dangerLevel = _calculateUvDanger(uvIndex);
    setState(() {
      _uvDangerLevel = dangerLevel;
    });
    _storageService.saveCalculation('УФ-индекс: $uvIndex, Опасность: $dangerLevel');
    _loadHistory();
  }

  Future<void> _loadHistory() async {
    final history = await _storageService.getCalculationHistory();
    setState(() {
      _history = history;
    });
  }

  String _calculateUvDanger(double uvIndex) {
    if (uvIndex <= 2) return 'Низкая';
    if (uvIndex <= 5) return 'Умеренная';
    if (uvIndex <= 7) return 'Высокая';
    if (uvIndex <= 10) return 'Очень высокая';
    return 'Экстремальная';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Расчет УФ-индекса')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          children: [
            Text('Выберите УФ-индекс:', style: TextStyle(fontSize: 16)),
            Slider(
              value: _selectedUvIndex,
              min: 0,
              max: 15,
              divisions: 15,
              label: _selectedUvIndex.round().toString(),
              onChanged: (double value) {
                setState(() {
                  _selectedUvIndex = value;
                });
              },
            ),
            SizedBox(height: 16),
            ElevatedButton(
              onPressed: () {
                _calculateAndSaveUvDanger(_selectedUvIndex);
              },
              child: Text('Рассчитать опасность'),
            ),
            SizedBox(height: 16),
            Text(
              'Уровень опасности: $_uvDangerLevel',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('История расчетов', style: TextStyle(fontSize: 20)),
                if (_history.isNotEmpty)
                  TextButton(
                    onPressed: () async {
                      await _storageService.clearCalculationHistory();
                      _loadHistory();
                    },
                    child: Text('Очистить'),
                  ),
              ],
            ),
            Expanded(
              child: ListView.builder(
                itemCount: _history.length,
                itemBuilder: (context, index) {
                  return ListTile(title: Text(_history[index]));
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}