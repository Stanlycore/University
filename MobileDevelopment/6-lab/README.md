# Лабораторная работа №6: Взаимодействие с сетью

Данный проект демонстрирует реализацию взаимодействия с REST API NASA в приложении "Фотографии марсохода Curiosity" с использованием BLoC-паттерна и Cubit в рамках шестого задания по мобильной разработке.


## Выполненные шаги

### 1. Инициализация проекта с зависимостями
- Добавлены зависимости `flutter_bloc` и `http` в файл `pubspec.yaml` для работы с Cubit и HTTP-запросами:
  ```yaml
  dependencies:
    flutter:
      sdk: flutter
    flutter_bloc: ^7.2.0
    http: ^0.13.5
  ```

### 2. Создание структуры проекта
- Создана папка `lib`, содержащая:
  - Подпапку `cubit` с файлами `nasa_cubit.dart` и `nasa_state.dart`.
  - Подпапку `models` с файлами `camera_model.dart`, `nasa_model.dart`, `photos_model.dart`, и `rover_model.dart`.
  - Подпапку `requests` с файлом `nasa_api.dart`.
  - Подпапку `screens` с файлом `photos_screen.dart`.
  - Файл `main.dart`.

### 3. Реализация взаимодействия с API
- Настроен HTTP-запрос к API NASA для получения фотографий марсохода Curiosity за Sol 100 с использованием библиотеки `http`.
- Создан файл `nasa_api.dart` для выполнения запроса и обработки ответа.

### 4. Реализация Cubit и состояний
- Реализованы состояния загрузки, успешной загрузки и ошибки в `nasa_state.dart`.
- Реализован `NasaCubit` в `nasa_cubit.dart` для управления загрузкой данных и их обработкой.

### 5. Реализация основного экрана
- **Файл `photos_screen.dart`:** Создан экран с использованием `BlocBuilder` для отображения данных. Важные части интерфейса:
  - Индикатор загрузки при начальном состоянии:
    ```dart
    if (state is NasaLoadingState) {
      return const Center(child: CircularProgressIndicator(color: Colors.white));
    }
    ```
  - Сообщение об ошибке с кнопкой повтора:
    ```dart
    if (state is NasaErrorState) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline, color: Colors.redAccent, size: 60),
            const Text('Не удалось загрузить фотографии!', style: TextStyle(color: Colors.white, fontSize: 18)),
            ElevatedButton(
              onPressed: () => context.read<NasaCubit>().loadData(),
              child: const Text('Повторить', style: TextStyle(color: Colors.white)),
            ),
          ],
        ),
      );
    }
    ```
  - Сетка фотографий при успешной загрузке:
    ```dart
    if (state is NasaLoadedState) {
      return GridView.builder(
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 2),
        itemBuilder: (context, index) {
          return Card(child: Image.network(state.data.photos![index].imgSrc ?? ''));
        },
      );
    }
    ```