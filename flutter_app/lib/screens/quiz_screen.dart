import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/stage.dart';
import '../services/stage_service.dart';
import '../theme/app_theme.dart';
import '../widgets/app_dialogs.dart';
import 'stage_summary_screen.dart';

class QuizScreen extends StatefulWidget {
  const QuizScreen({
    super.key,
    required this.player,
    required this.stage,
    this.restart = false,
  });

  final Player player;
  final Stage stage;
  final bool restart;

  @override
  State<QuizScreen> createState() => _QuizScreenState();
}

class _QuizScreenState extends State<QuizScreen> {
  final _stageService = StageService();
  StageAttempt? _attempt;
  final Set<int> _answeredQuestionIds = {};
  int _questionIndex = 0;
  bool _isLoading = true;
  bool _isBusy = false;
  bool? _isCorrect;
  String? _hint1;
  String? _hint2;
  bool _hint2Used = false;
  String? _errorMessage;

  AttemptQuestion? get _currentQuestion {
    final attempt = _attempt;
    if (attempt == null || _questionIndex >= attempt.questions.length) {
      return null;
    }
    return attempt.questions[_questionIndex];
  }

  @override
  void initState() {
    super.initState();
    _startStage();
  }

  @override
  void dispose() {
    _stageService.dispose();
    super.dispose();
  }

  Future<void> _startStage() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });
    try {
      final attempt = await _stageService.startStage(
        widget.stage.id,
        playerId: widget.player.id,
        restart: widget.restart,
      );
      if (!mounted) return;
      _answeredQuestionIds
        ..clear()
        ..addAll(
          attempt.questions
              .where((question) => question.isAnswered)
              .map((question) => question.questionId),
        );
      final nextIndex = attempt.questions.indexWhere(
        (question) => !_answeredQuestionIds.contains(question.questionId),
      );
      setState(() {
        _attempt = attempt;
        _questionIndex = nextIndex == -1 ? attempt.questions.length : nextIndex;
        _isLoading = false;
      });
      if (nextIndex == -1) _finishStage();
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

  Future<void> _submitAnswer(String option) async {
    final attempt = _attempt;
    final question = _currentQuestion;
    if (attempt == null || question == null || _isBusy || _isCorrect != null) {
      return;
    }
    setState(() {
      _isBusy = true;
      _errorMessage = null;
    });
    try {
      final result = await _stageService.submitAnswer(
        attempt.attemptId,
        questionId: question.questionId,
        selectedOption: option,
      );
      if (!mounted) return;
      setState(() {
        _isBusy = false;
        _isCorrect = result.isCorrect;
        _hint1 = result.hint1;
        _answeredQuestionIds.add(question.questionId);
      });
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  Future<void> _showNextHint() async {
    final attempt = _attempt;
    final question = _currentQuestion;
    if (attempt == null ||
        question == null ||
        _isBusy ||
        _isCorrect != false ||
        _hint2Used) {
      return;
    }
    setState(() {
      _isBusy = true;
      _errorMessage = null;
    });
    try {
      final hint = await _stageService.getNextHint(
        attempt.attemptId,
        questionId: question.questionId,
      );
      if (!mounted) return;
      setState(() {
        _isBusy = false;
        _hint2 = hint.hint2;
        _hint2Used = true;
      });
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  Future<void> _continue() async {
    final attempt = _attempt;
    if (attempt == null || _isBusy) return;
    final nextIndex = attempt.questions.indexWhere(
      (question) =>
          !_answeredQuestionIds.contains(question.questionId) &&
          question.orderNo > (_currentQuestion?.orderNo ?? 0),
    );
    if (nextIndex == -1) {
      await _finishStage();
      return;
    }
    setState(() {
      _questionIndex = nextIndex;
      _isCorrect = null;
      _hint1 = null;
      _hint2 = null;
      _hint2Used = false;
      _errorMessage = null;
    });
  }

  Future<void> _finishStage() async {
    final attempt = _attempt;
    if (attempt == null || _isBusy) return;
    setState(() {
      _isBusy = true;
      _errorMessage = null;
    });
    try {
      final summary = await _stageService.finishStage(attempt.attemptId);
      if (!mounted) return;
      await Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) =>
              StageSummaryScreen(stage: widget.stage, summary: summary),
        ),
      );
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    } on FormatException catch (error) {
      if (mounted) {
        setState(() {
          _isBusy = false;
          _errorMessage = error.message;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(widget.stage.name)),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _errorMessage != null && _attempt == null
          ? _buildStartError()
          : _buildQuestion(),
    );
  }

  Widget _buildQuestion() {
    final attempt = _attempt;
    final question = _currentQuestion;
    if (attempt == null || question == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Text(
          'Câu ${question.orderNo} / ${attempt.questions.length}',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 16),
        Text(question.content, style: Theme.of(context).textTheme.titleLarge),
        const SizedBox(height: 20),
        for (var index = 0; index < question.options.length; index++) ...[
          OutlinedButton(
            onPressed: _isBusy || _isCorrect != null
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
        if (_isCorrect != null) ...[
          const SizedBox(height: 8),
          Text(
            _isCorrect! ? 'Chính xác!' : 'Chưa chính xác.',
            style: TextStyle(
              color: _isCorrect! ? AppColors.success : AppColors.error,
              fontWeight: FontWeight.bold,
            ),
          ),
          if (!_isCorrect! && _hint1 != null) ...[
            const SizedBox(height: 8),
            Text('Hint 1: $_hint1'),
          ],
          if (!_isCorrect! && !_hint2Used) ...[
            const SizedBox(height: 8),
            OutlinedButton(
              onPressed: _isBusy ? null : _showNextHint,
              child: const Text('Xem tiếp (Hint 2, -60 EXP)'),
            ),
          ],
          if (_hint2 != null) ...[
            const SizedBox(height: 8),
            Text('Hint 2: $_hint2'),
          ],
          const SizedBox(height: 16),
          FilledButton(
            onPressed: _isBusy ? null : _continue,
            child: Text(
              _answeredQuestionIds.length == attempt.questions.length
                  ? 'Hoàn thành ải'
                  : 'Câu tiếp theo',
            ),
          ),
        ],
        if (_errorMessage != null) ...[
          const SizedBox(height: 12),
          Text(_errorMessage!, style: const TextStyle(color: AppColors.error)),
        ],
        if (_isBusy) ...[
          const SizedBox(height: 16),
          const Center(child: CircularProgressIndicator()),
        ],
      ],
    );
  }

  Widget _buildStartError() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(_errorMessage!, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(onPressed: _startStage, child: const Text('Thử lại')),
          ],
        ),
      ),
    );
  }
}
