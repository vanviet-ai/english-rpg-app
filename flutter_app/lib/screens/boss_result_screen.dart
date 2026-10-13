import 'package:confetti/confetti.dart';
import 'package:flutter/material.dart';

import '../models/stage.dart';
import '../services/stage_service.dart';
import '../theme/app_theme.dart';
import '../widgets/app_dialogs.dart';

class BossResultScreen extends StatefulWidget {
  const BossResultScreen({
    super.key,
    required this.stage,
    required this.result,
  });

  final Stage stage;
  final BossFinishResult result;

  @override
  State<BossResultScreen> createState() => _BossResultScreenState();
}

class _BossResultScreenState extends State<BossResultScreen> {
  final _stageService = StageService();
  late final ConfettiController _confettiController;
  BossAttemptSummary? _summary;
  String? _errorMessage;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _confettiController = ConfettiController(
      duration: const Duration(milliseconds: 4500),
    );
    if (widget.result.isVictory) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) _confettiController.play();
      });
    }
    _loadSummary();
  }

  @override
  void dispose() {
    _confettiController.dispose();
    _stageService.dispose();
    super.dispose();
  }

  Future<void> _loadSummary() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });
    try {
      final summary = await _stageService.getBossSummary(
        widget.result.attemptId,
      );
      if (mounted) {
        setState(() {
          _summary = summary;
          _isLoading = false;
        });
      }
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isLoading = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final result = widget.result;
    return Scaffold(
      appBar: AppBar(title: const Text('Kết quả Boss')),
      body: Stack(
        children: [
          ListView(
            padding: const EdgeInsets.all(20),
            children: [
              Text(
                result.isVictory ? 'Chiến thắng!' : 'Chưa chiến thắng',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                  color: result.isVictory ? AppColors.success : AppColors.error,
                ),
              ),
              const SizedBox(height: 12),
              Text(
                'Số câu đúng: ${result.correctCount}/15',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.titleLarge,
              ),
              const SizedBox(height: 8),
              if (result.isVictory && result.expEarned > 0)
                Text(
                  'Nhận ${result.expEarned} EXP',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    color: AppColors.exp,
                    fontWeight: FontWeight.w600,
                  ),
                )
              else if (result.isVictory)
                const Text(
                  'Chỉ ôn tập, không tính điểm.',
                  textAlign: TextAlign.center,
                )
              else
                const Text(
                  'Chưa chiến thắng, thử lại nhé!',
                  textAlign: TextAlign.center,
                ),
              const SizedBox(height: 24),
              Text(
                'Đáp án và giải thích',
                style: Theme.of(context).textTheme.titleLarge,
              ),
              const SizedBox(height: 8),
              if (_isLoading)
                const Center(child: CircularProgressIndicator())
              else if (_errorMessage != null) ...[
                Text(
                  _errorMessage!,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
                const SizedBox(height: 8),
                OutlinedButton(
                  onPressed: _loadSummary,
                  child: const Text('Tải lại đáp án'),
                ),
              ] else if (_summary case final summary?)
                for (final question in summary.questions)
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
                          Text(
                            'Bạn chọn: ${question.selectedOption ?? 'Chưa trả lời'}',
                          ),
                          Text('Đáp án đúng: ${question.correctOption}'),
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
          if (result.isVictory)
            Positioned.fill(
              child: IgnorePointer(
                child: ConfettiWidget(
                  confettiController: _confettiController,
                  blastDirectionality: BlastDirectionality.explosive,
                  numberOfParticles: 32,
                  emissionFrequency: 0.08,
                  maxBlastForce: 28,
                  minBlastForce: 12,
                  gravity: 0.1,
                ),
              ),
            ),
        ],
      ),
    );
  }
}
