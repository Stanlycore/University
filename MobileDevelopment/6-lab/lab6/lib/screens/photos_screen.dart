import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '/cubit/nasa_cubit.dart';
import '/cubit/nasa_state.dart';
import '/models/photos_model.dart';

// Главный экран для отображения фотографий марсохода Curiosity
class PhotosScreen extends StatelessWidget {
  const PhotosScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Фотографии Curiosity - Sol 100'),
        centerTitle: true,
        backgroundColor: const Color.fromARGB(255, 232, 219, 255),
      ),
      body: Container(
        child: BlocProvider(
          create: (context) => NasaCubit()..loadData(),
          child: BlocBuilder<NasaCubit, NasaState>(
            builder: (context, state) {
              // Отображение индикатора загрузки во время получения данных
              if (state is NasaLoadingState) {
                return const Center(
                  child: CircularProgressIndicator(
                    color: Colors.white,
                  ),
                );
              }
              // Отображение сообщения об ошибке и кнопки для повторной попытки
              else if (state is NasaErrorState) {
                return Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        Icons.error_outline,
                        color: Colors.redAccent,
                        size: 60,
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'Не удалось загрузить фотографии!',
                        style: TextStyle(color: Colors.white, fontSize: 18),
                      ),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: () => context.read<NasaCubit>().loadData(),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.deepPurpleAccent,
                          padding: const EdgeInsets.symmetric(
                              horizontal: 20, vertical: 10),
                        ),
                        child: const Text(
                          'Повторить',
                          style: TextStyle(color: Colors.white),
                        ),
                      ),
                    ],
                  ),
                );
              }
              // Отображение фотографий в виде сетки при успешной загрузке
              else if (state is NasaLoadedState) {
                final photos = state.data.photos;
                if (photos == null || photos.isEmpty) {
                  return const Center(
                    child: Text(
                      'Фотографии отсутствуют',
                      style: TextStyle(color: Colors.white, fontSize: 18),
                    ),
                  );
                }
                return GridView.builder(
                  padding: const EdgeInsets.all(8),
                  gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 2,
                    crossAxisSpacing: 8,
                    mainAxisSpacing: 8,
                    childAspectRatio: 0.7,
                  ),
                  itemCount: photos.length,
                  itemBuilder: (context, index) {
                    final photo = photos[index];
                    return _buildPhotoCard(context, photo);
                  },
                );
              }
              return const SizedBox.shrink();
            },
          ),
        ),
      ),
    );
  }

  // Виджет для создания карточки с фотографией
  Widget _buildPhotoCard(BuildContext context, Photos photo) {
    return Card(
      color: Colors.white.withOpacity(0.1),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
      ),
      child: InkWell(
        onTap: () {
          // Показ диалога с деталями фотографии при нажатии
          _showPhotoDetails(context, photo);
        },
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: ClipRRect(
                borderRadius: const BorderRadius.vertical(
                  top: Radius.circular(12),
                ),
                child: SizedBox(
                  width: double.infinity,
                  height: 150,
                  child: Image.network(         ///////////////////////////////////////////////////////////////////////////
                    photo.imgSrc ?? '',
                    fit: BoxFit.cover,
                    width: double.infinity,
                    height: 150,
                    loadingBuilder: (
                      BuildContext context,
                      Widget child,
                      ImageChunkEvent? loadingProgress,
                    ) {
                      if (loadingProgress == null) return child;
                      return Center(
                        child: CircularProgressIndicator(
                          value: loadingProgress.expectedTotalBytes != null
                              ? loadingProgress.cumulativeBytesLoaded /
                                  loadingProgress.expectedTotalBytes!
                              : null,
                          color: Colors.white,
                        ),
                      );
                    },
                    errorBuilder: (
                      BuildContext context,
                      Object error,
                      StackTrace? stackTrace,
                    ) {
                      return Container(
                        color: Colors.grey[800],
                        child: const Icon(
                          Icons.broken_image,
                          color: Colors.white,
                          size: 50,
                        ),
                      );
                    },
                  ),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Сол: ${photo.sol ?? 'Н/Д'}',
                    style: const TextStyle(color: Color.fromARGB(255, 38, 38, 38), fontSize: 14),
                  ),
                  Text(
                    'Камера: ${photo.camera?.name ?? 'Н/Д'}',
                    style: const TextStyle(color: Color.fromARGB(255, 38, 38, 38), fontSize: 12),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Метод для отображения диалога с деталями фотографии
  void _showPhotoDetails(BuildContext context, Photos photo) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        backgroundColor: Colors.black87,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        title: const Text(
          'Детали фотографии',
          style: TextStyle(color: Colors.white),
        ),
        content: SizedBox(
          width: double.maxFinite, // Ограничиваем ширину диалога
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'ID: ${photo.id ?? 'Н/Д'}',
                  style: const TextStyle(color: Colors.white),
                ),
                Text(
                  'Дата на Земле: ${photo.earthDate ?? 'Н/Д'}',
                  style: const TextStyle(color: Colors.white),
                ),
                Text(
                  'Камера: ${photo.camera?.fullName ?? 'Н/Д'}',
                  style: const TextStyle(color: Colors.white),
                ),
                Text(
                  'Марсоход: ${photo.rover?.name ?? 'Н/Д'}',
                  style: const TextStyle(color: Colors.white),
                ),
                Text(
                  'Статус марсохода: ${photo.rover?.status ?? 'Н/Д'}',
                  style: const TextStyle(color: Colors.white),
                ),
              ],
            ),
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text(
              'Закрыть',
              style: TextStyle(color: Colors.white),
            ),
          ),
        ],
      ),
    );
  }
}