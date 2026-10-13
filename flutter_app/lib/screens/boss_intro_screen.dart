import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/stage.dart';
import '../services/stage_service.dart';
import '../widgets/app_dialogs.dart';
import 'boss_quiz_screen.dart';

class BossIntroScreen extends StatefulWidget {
  const BossIntroScreen({super.key, required this.player, required this.stage});

  final Player player;
  final Stage stage;

  @override
  State<BossIntroScreen> createState() => _BossIntroScreenState();
}

class _BossIntroScreenState extends State<BossIntroScreen> {
  final _stageService = StageService();
  bool _isStarting = false;
  String? _errorMessage;

  @override
  void dispose() {
    _stageService.dispose();
    super.dispose();
  }

  Future<void> _startBoss() async {
    if (_isStarting) return;
    setState(() {
      _isStarting = true;
      _errorMessage = null;
    });
    try {
      final attempt = await _stageService.startBossStage(
        playerId: widget.player.id,
      );
      if (!mounted) return;
      await Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) => BossQuizScreen(stage: widget.stage, attempt: attempt),
        ),
      );
      if (mounted) setState(() => _isStarting = false);
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isStarting = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isStarting = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Giới thiệu Boss')),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 520),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  widget.stage.name,
                  style: Theme.of(context).textTheme.headlineMedium,
                  textAlign: TextAlign.center,
                ),
                const SizedBox(height: 24),
                const Card(
                  child: Padding(
                    padding: EdgeInsets.all(20),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('Luật chơi', style: TextStyle(fontSize: 20)),
                        SizedBox(height: 12),
                        Text('• 15 câu hỏi trong 30 phút.'),
                        SizedBox(height: 8),
                        Text('• Thắng khi trả lời đúng ít nhất 13/15 câu.'),
                        SizedBox(height: 8),
                        Text('• Thắng lần đầu nhận thưởng 2000 EXP.'),
                        SizedBox(height: 8),
                        Text('• Chơi lại chỉ để ôn tập, không tính điểm.'),
                        SizedBox(height: 8),
                        Text('• Đáp án chỉ hiện sau khi nộp bài.'),
                      ],
                    ),
                  ),
                ),
                if (_errorMessage != null) ...[
                  const SizedBox(height: 12),
                  Text(
                    _errorMessage!,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      color: Theme.of(context).colorScheme.error,
                    ),
                  ),
                ],
                const SizedBox(height: 20),
                FilledButton(
                  onPressed: _isStarting ? null : _startBoss,
                  child: _isStarting
                      ? const SizedBox.square(
                          dimension: 20,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                      : const Text('Bắt đầu'),
                ),
                OutlinedButton(
                  onPressed: _isStarting
                      ? null
                      : () => Navigator.of(context).pop(),
                  child: const Text('Quay lại'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
