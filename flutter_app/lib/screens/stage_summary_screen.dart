import 'package:confetti/confetti.dart';
import 'package:flutter/material.dart';

import '../models/stage.dart';
import '../theme/app_theme.dart';

class StageSummaryScreen extends StatefulWidget {
  const StageSummaryScreen({
    super.key,
    required this.stage,
    required this.summary,
  });

  final Stage stage;
  final StageSummary summary;

  @override
  State<StageSummaryScreen> createState() => _StageSummaryScreenState();
}

class _StageSummaryScreenState extends State<StageSummaryScreen> {
  late final ConfettiController _confettiController;

  bool get _passed => widget.summary.correctCount >= 8;

  @override
  void initState() {
    super.initState();
    _confettiController = ConfettiController(
      duration: const Duration(milliseconds: 2500),
    );
    if (_passed) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) _confettiController.play();
      });
    }
  }

  @override
  void dispose() {
    _confettiController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final stage = widget.stage;
    final summary = widget.summary;
    return Scaffold(
      appBar: AppBar(title: const Text('Tổng kết ải')),
      body: Stack(
        children: [
          ListView(
            padding: const EdgeInsets.all(20),
            children: [
              Text(stage.name, style: Theme.of(context).textTheme.headlineSmall),
              const SizedBox(height: 16),
              Text(
                'Số câu đúng: ${summary.correctCount}',
                style: const TextStyle(color: AppColors.success),
              ),
              if (!_passed)
                const Padding(
                  padding: EdgeInsets.only(top: 8),
                  child: Text('Cố gắng thêm nhé, bạn sẽ vượt qua ở lần sau!'),
                ),
              Text(
                'Số câu sai: ${summary.incorrectCount}',
                style: const TextStyle(color: AppColors.error),
              ),
              Text(
                'EXP nhận được: ${summary.expEarned}',
                style: const TextStyle(
                  color: AppColors.exp,
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 20),
              Text(
                'Ôn tập câu sai',
                style: Theme.of(context).textTheme.titleLarge,
              ),
              const SizedBox(height: 8),
              if (summary.incorrectQuestions.isEmpty)
                const Text('Không có câu sai. Làm tốt lắm!')
              else
                for (final question in summary.incorrectQuestions)
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Câu ${question.orderNo}: ${question.content}',
                            style: Theme.of(context).textTheme.titleMedium,
                          ),
                          const SizedBox(height: 8),
                          Text('Bạn chọn: ${question.selectedOption}'),
                          Text('Đáp án đúng: ${question.correctOption}'),
                          if (question.hint1 != null)
                            Text('Hint 1: ${question.hint1}'),
                          if (question.hint2 != null)
                            Text('Hint 2: ${question.hint2}'),
                          if (question.explanation != null)
                            Text('Giải thích: ${question.explanation}'),
                        ],
                      ),
                    ),
                  ),
              const SizedBox(height: 16),
              FilledButton(
                onPressed: () =>
                    Navigator.of(context).popUntil((route) => route.isFirst),
                child: const Text('Về Trang chủ'),
              ),
            ],
          ),
          if (_passed)
            Positioned.fill(
              child: IgnorePointer(
                child: ConfettiWidget(
                  confettiController: _confettiController,
                  blastDirectionality: BlastDirectionality.explosive,
                  numberOfParticles: 20,
                  emissionFrequency: 0.06,
                  maxBlastForce: 18,
                  minBlastForce: 8,
                  gravity: 0.12,
                ),
              ),
            ),
        ],
      ),
    );
  }
}
