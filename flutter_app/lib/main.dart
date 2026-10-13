import 'package:flutter/material.dart';

import 'screens/login_screen.dart';

void main() {
  runApp(const EnglishRpgApp());
}

class EnglishRpgApp extends StatelessWidget {
  const EnglishRpgApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'English RPG',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.indigo),
        useMaterial3: true,
      ),
      home: const LoginScreen(),
    );
  }
}
