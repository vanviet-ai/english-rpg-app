import 'dart:async';

import 'package:flutter/material.dart';

import '../models/stage.dart';
import '../services/stage_service.dart';
import '../theme/app_theme.dart';
import '../widgets/app_dialogs.dart';
import 'boss_result_screen.dart';

class BossQuizScreen extends StatefulWidget {
  const BossQuizScreen({super.key, required this.stage, required this.attempt});

  final Stage stage;
  final StageAttempt attempt;

  @override
  State<BossQuizScreen> createState() => _BossQuizScreenState();
}

class _BossQuizScreenState extends State<BossQuizScreen> {
  final _stageService = StageService();
  final _countdown = Stopwatch();
  Timer? _ticker;
  late Duration _remaining;
  int _questionIndex = 0;
  final Set<int> _answeredQuestionIds = {};
  bool _isSubmitting = false;
  bool _isFinishing = false;
  bool _timeExpired = false;
  String? _errorMessage;

  AttemptQuestion? get _currentQuestion {
    if (_questionIndex >= widget.attempt.questions.length) return null;
    return widget.attempt.questions[_questionIndex];
  }

  @override
  void initState() {
    super.initState();
    _remaining = widget.attempt.remainingAtResponse ?? Duration.zero;
    _countdown.start();
    _ticker = Timer.periodic(const Duration(seconds: 1), (_) {
      final nextRemaining =
          (widget.attempt.remainingAtResponse ?? Duration.zero) -
          _countdown.elapsed;
      if (nextRemaining <= Duration.zero) {
        _ticker?.cancel();
        if (mounted) {
          setState(() {
            _remaining = Duration.zero;
            _timeExpired = true;
          });
        }
        if (!_isSubmitting) _finishBoss();
      } else if (mounted) {
        setState(() => _remaining = nextRemaining);
      }
    });
    if (_remaining <= Duration.zero) {
      WidgetsBinding.instance.addPostFrameCallback((_) => _finishBoss());
    }
  }

  @override
  void dispose() {
    _ticker?.cancel();
    _countdown.stop();
    _stageService.dispose();
    super.dispose();
  }

  Future<void> _submitAnswer(String option) async {
    final question = _currentQuestion;
    if (question == null || _isSubmitting || _isFinishing || _timeExpired) {
      return;
    }
    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
    });
    try {
      await _stageService.submitBossAnswer(
        widget.attempt.attemptId,
        questionId: question.questionId,
        selectedOption: option,
      );
      await Future<void>.delayed(const Duration(milliseconds: 500));
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
        _answeredQuestionIds.add(question.questionId);
        if (!_timeExpired &&
            _questionIndex < widget.attempt.questions.length - 1) {
          _questionIndex++;
        }
      });
      if (_timeExpired) await _finishBoss();
    } on StageServiceException catch (error) {
      if (!mounted) return;
      if (error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
        _errorMessage = error.message;
      });
      if (_timeExpired) await _finishBoss();
    } on FormatException catch (error) {
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
        _errorMessage = error.message;
      });
      if (_timeExpired) await _finishBoss();
    }
  }

  Future<void> _finishBoss() async {
    if (_isFinishing || _isSubmitting) return;
    setState(() {
      _isFinishing = true;
      _errorMessage = null;
    });
    try {
      final result = await _stageService.finishBoss(widget.attempt.attemptId);
      if (!mounted) return;
      _ticker?.cancel();
      await Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) => BossResultScreen(stage: widget.stage, result: result),
        ),
      );
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isFinishing = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isFinishing = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  String get _formattedTime {
    final totalSeconds = _remaining.inSeconds;
    final minutes = (totalSeconds ~/ 60).toString().padLeft(2, '0');
    final seconds = (totalSeconds % 60).toString().padLeft(2, '0');
    return '$minutes:$seconds';
  }

  @override
  Widget build(BuildContext context) {
    final question = _currentQuestion;
    final isQuestionAnswered =
        question != null && _answeredQuestionIds.contains(question.questionId);
    return Scaffold(
      appBar: AppBar(title: Text(widget.stage.name)),
      body: question == null
          ? const Center(child: Text('Không có câu hỏi trong lượt Boss.'))
          : ListView(
              padding: const EdgeInsets.all(20),
              children: [
                Text(
                  'Còn lại: $_formattedTime',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                    color: _remaining.inMinutes < 5 ? AppColors.error : null,
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  'Câu ${question.orderNo} / ${widget.attempt.questions.length}',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: 16),
                Text(
                  question.content,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: 20),
                for (
                  var index = 0;
                  index < question.options.length;
                  index++
                ) ...[
                  OutlinedButton(
                    onPressed:
                        _isSubmitting ||
                            _isFinishing ||
                            _timeExpired ||
                            isQuestionAnswered
                        ? null
                        : () => _submitAnswer(String.fromCharCode(65 + index)),
                    child: Align(
                      alignment: Alignment.centerLeft,
                      child: Text(
                        '${String.fromCharCode(65 + index)}. ${question.options[index]}',
                      ),
                    ),
                  ),
                  const SizedBox(height: 8),
                ],
                if (isQuestionAnswered) ...[
                  const SizedBox(height: 8),
                  const Text(
                    'Đã ghi nhận câu trả lời. Kết quả sẽ hiện sau khi nộp bài.',
                  ),
                ],
                if (_isSubmitting || _isFinishing) ...[
                  const SizedBox(height: 8),
                  const Center(child: CircularProgressIndicator()),
                ],
                if (_errorMessage != null) ...[
                  const SizedBox(height: 12),
                  Text(
                    _errorMessage!,
                    style: TextStyle(color: AppColors.error),
                  ),
                ],
                const SizedBox(height: 16),
                FilledButton(
                  onPressed: _isSubmitting || _isFinishing ? null : _finishBoss,
                  child: Text(_timeExpired ? 'Thử nộp lại' : 'Nộp bài'),
                ),
              ],
            ),
    );
  }
}
