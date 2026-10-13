import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/stage.dart';
import '../services/stage_service.dart';
import '../widgets/app_dialogs.dart';
import 'boss_intro_screen.dart';
import 'quiz_screen.dart';

class StagesMapScreen extends StatefulWidget {
  const StagesMapScreen({super.key, required this.player});

  final Player player;

  @override
  State<StagesMapScreen> createState() => _StagesMapScreenState();
}

class _StagesMapScreenState extends State<StagesMapScreen> {
  final _stageService = StageService();
  List<Stage>? _stages;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _loadStages();
  }

  @override
  void dispose() {
    _stageService.dispose();
    super.dispose();
  }

  Future<void> _loadStages() async {
    setState(() => _errorMessage = null);
    try {
      final stages = await _stageService.getStages(widget.player.id);
      if (mounted) setState(() => _stages = stages);
    } on StageServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) setState(() => _errorMessage = error.message);
    } on FormatException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    }
  }

  Future<void> _openStage(Stage stage) async {
    if (stage.isLocked) return;
    var restart = false;
    if (stage.isNormal && stage.hasInProgressAttempt) {
      final shouldRestart = await showResumeStageDialog(context);
      if (shouldRestart == null || !mounted) return;
      restart = shouldRestart;
    }
    await Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => stage.isNormal
            ? QuizScreen(player: widget.player, stage: stage, restart: restart)
            : BossIntroScreen(player: widget.player, stage: stage),
      ),
    );
    if (mounted) _loadStages();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Bản đồ ải')),
      body: _stages == null
          ? _buildLoadingOrError()
          : RefreshIndicator(
              onRefresh: _loadStages,
              child: ListView.separated(
                padding: const EdgeInsets.all(16),
                itemCount: _stages!.length,
                separatorBuilder: (_, _) => const SizedBox(height: 8),
                itemBuilder: (context, index) =>
                    _buildStageCard(_stages![index]),
              ),
            ),
    );
  }

  Widget _buildStageCard(Stage stage) {
    final isBoss = !stage.isNormal;
    final isEnabled = !stage.isLocked;
    final subtitle = stage.isLocked
        ? 'Chưa mở khóa'
        : isBoss
        ? 'Boss đã mở khóa · Chạm để xem luật chơi'
        : stage.status == 'completed'
        ? 'Đã hoàn thành · Chạm để chơi lại'
        : stage.hasInProgressAttempt
        ? 'Có lượt chơi đang tiếp tục'
        : 'Đã mở khóa · Chạm để bắt đầu';

    return Opacity(
      opacity: stage.isLocked ? 0.48 : 1,
      child: Card(
        child: ListTile(
          enabled: isEnabled,
          leading: Icon(
            stage.isLocked
                ? Icons.lock_outline
                : isBoss
                ? Icons.shield_outlined
                : stage.status == 'completed'
                ? Icons.check_circle_outline
                : Icons.flag_outlined,
          ),
          title: Text(stage.name),
          subtitle: Text(subtitle),
          trailing: isEnabled ? const Icon(Icons.chevron_right) : null,
          onTap: isEnabled ? () => _openStage(stage) : null,
        ),
      ),
    );
  }

  Widget _buildLoadingOrError() {
    if (_errorMessage == null) {
      return const Center(child: CircularProgressIndicator());
    }
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(_errorMessage!, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(onPressed: _loadStages, child: const Text('Thử lại')),
          ],
        ),
      ),
    );
  }
}
