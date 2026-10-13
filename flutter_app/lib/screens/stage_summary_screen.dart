import 'package:flutter/material.dart';

import '../models/stage.dart';
import '../theme/app_theme.dart';

class StageSummaryScreen extends StatelessWidget {
  const StageSummaryScreen({
    super.key,
    required this.stage,
    required this.summary,
  });

  final Stage stage;
  final StageSummary summary;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Tổng kết ải')),
      body: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          Text(stage.name, style: Theme.of(context).textTheme.headlineSmall),
          const SizedBox(height: 16),
          Text(
            'Số câu đúng: ${summary.correctCount}',
            style: const TextStyle(color: AppColors.success),
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
          Text('Ôn tập câu sai', style: Theme.of(context).textTheme.titleLarge),
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
    );
  }
}
