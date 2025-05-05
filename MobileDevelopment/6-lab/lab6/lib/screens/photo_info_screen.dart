import 'package:flutter/material.dart';
import '/models/photos_model.dart';

class PhotoInfoScreen extends StatelessWidget {
  final Photos photo;

  const PhotoInfoScreen({Key? key, required this.photo}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Детали фотки'), centerTitle: true),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Image.network(
                photo.imgSrc ?? '',
                fit: BoxFit.cover,
                width: double.infinity,
                height: 300,
                loadingBuilder: (
                  BuildContext context,
                  Widget child,
                  ImageChunkEvent? loadingProgress,
                ) {
                  if (loadingProgress == null) return child;
                  return Center(
                    child: CircularProgressIndicator(
                      value:
                          loadingProgress.expectedTotalBytes != null
                              ? loadingProgress.cumulativeBytesLoaded /
                                  loadingProgress.expectedTotalBytes!
                              : null,
                    ),
                  );
                },
                errorBuilder: (
                  BuildContext context,
                  Object error,
                  StackTrace? stackTrace,
                ) {
                  return const Icon(Icons.error);
                },
              ),
            ),
            const SizedBox(height: 20),
            _buildInfoCard('Информация о фотографии', [
              _buildInfoRow('ID', photo.id?.toString() ?? 'Неизвестно'),
              _buildInfoRow('Сол', photo.sol?.toString() ?? 'Неизвестно'),
              _buildInfoRow(
                'Дата на Земле',
                photo.earthDate ?? 'Дата недоступна',
              ),
            ]),
            const SizedBox(height: 20),
            _buildInfoCard('Информация о камере', [
              _buildInfoRow('Название', photo.camera?.name ?? 'Неизвестно'),
              _buildInfoRow(
                'Полное название',
                photo.camera?.fullName ?? 'Неизвестно',
              ),
              _buildInfoRow('ID', photo.camera?.id?.toString() ?? 'Неизвестно'),
            ]),
            const SizedBox(height: 20),
            _buildInfoCard('Информация о ровере', [
              _buildInfoRow('Название', photo.rover?.name ?? 'Неизвестно'),
              _buildInfoRow('Статус', photo.rover?.status ?? 'Неизвестно'),
              _buildInfoRow(
                'Дата запуска',
                photo.rover?.launchDate ?? 'Неизвестно',
              ),
              _buildInfoRow(
                'Дата посадки',
                photo.rover?.landingDate ?? 'Неизвестно',
              ),
            ]),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoCard(String title, List<Widget> children) {
    return Card(
      elevation: 4,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const Divider(),
            ...children,
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Text('$label: ', style: const TextStyle(fontWeight: FontWeight.bold)),
          Text(value),
        ],
      ),
    );
  }
}
