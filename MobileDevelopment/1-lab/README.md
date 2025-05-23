# Лабораторная работа №1: Настройка среды разработки Flutter

Данный проект представляет собой базовое приложение на фреймворке Flutter, созданное в рамках выполнения первой лабораторной работы по курсу мобильной разработки. Проект демонстрирует успешную настройку рабочей среды и базовые принципы работы с Flutter.

### 1. Установка и настройка Flutter
- Скачан и установлен Flutter SDK с официального сайта [flutter.dev](https://docs.flutter.dev/get-started/install)
- Добавлены переменные среды Windows для глобального доступа к Flutter
- Выполнена проверка конфигурации через команду:
  ```bash
  flutter doctor
  ```

### 2. Настройка Android SDK
- Установлен Android Studio [developer.android.com/studio](https://developer.android.com/studio)
- Через Android Studio установлен Android SDK и компоненты:
  - Android SDK Platform-Tools
  - Android SDK Command-line Tools
- Приняты лицензионные соглашения:
  ```bash
  flutter doctor --android-licenses
  ```

### 3. Интеграция с IDE
- Установлено расширение Flutter для Visual Studio Code
- Настроена интеграция с эмулятором Android:
  - Создан виртуальный девайс "Medium Phone API 35" через Android Studio

### 4. Создание и запуск проекта
- Создан новый проект через VS Code:
  ```bash
  Ctrl+Shift+P → Flutter: New Project
  ```
- Выполнен запуск приложения на эмуляторе:
  ```bash
  F5 (Start Debugging)
  ```

### 5. Работа с Git
- Инициализирован репозиторий:
  ```bash
  git init
  ```
- Создан и отправлен начальный коммит:
  ```bash
  git add .
  git commit -m "first commit"
  git remote add origin https://github.com/Stanlycore/University.git
  git push -u origin master
  ```
- Установлен Git Extensions для удобства работы с репозиторием

## Проверка конфигурации
Итоговый статус проверки `flutter doctor`:
```
[√] Flutter (Channel stable, 3.13.9)
[√] Android toolchain
[√] Android Studio
[!] VS Code (Visual Studio не требуется для текущей конфигурации)
[√] Connected device (1 available)
```