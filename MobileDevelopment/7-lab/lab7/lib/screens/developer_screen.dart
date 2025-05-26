import 'package:flutter/material.dart';

class DeveloperScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('О разработчике')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Имя: Непомнящих Станислав', style: TextStyle(fontSize: 20)),
            Text('Группа: ИВТ-22', style: TextStyle(fontSize: 16)),
            Text('Email: cnfybckfd24@mail.ru', style: TextStyle(fontSize: 16)),
          ],
        ),
      ),
    );
  }
}