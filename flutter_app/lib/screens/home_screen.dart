import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/university.dart';
import 'stages_map_screen.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key, required this.player, required this.university});

  final Player player;
  final University university;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Trang chủ')),
      body: Center(
        child: Card(
          margin: const EdgeInsets.all(24),
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Xin chào, ${player.name}!',
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
                const SizedBox(height: 16),
                Text(
                  'Trường mục tiêu: ${university.name} (${university.shortName})',
                ),
                const SizedBox(height: 8),
                Text('EXP hiện tại: ${player.totalExp}'),
                const SizedBox(height: 20),
                FilledButton(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute<void>(
                        builder: (_) => StagesMapScreen(player: player),
                      ),
                    );
                  },
                  child: const Text('Vào bản đồ ải'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
