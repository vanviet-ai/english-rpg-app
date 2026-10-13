import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/university.dart';
import '../services/player_service.dart';
import '../theme/app_theme.dart';
import '../widgets/app_dialogs.dart';
import 'stages_map_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key, required this.player, required this.university});

  final Player player;
  final University university;

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _playerService = PlayerService();
  late Player _player;
  String? _errorMessage;
  bool _isRefreshing = false;
  bool _unlockDialogShown = false;

  @override
  void initState() {
    super.initState();
    _player = widget.player;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _showUnlockDialogIfNeeded();
    });
  }

  @override
  void dispose() {
    _playerService.dispose();
    super.dispose();
  }

  Future<void> _openStagesMap() async {
    await Navigator.of(context).push(
      MaterialPageRoute<void>(builder: (_) => StagesMapScreen(player: _player)),
    );
    if (mounted) await _refreshPlayer();
  }

  Future<void> _refreshPlayer() async {
    setState(() {
      _isRefreshing = true;
      _errorMessage = null;
    });

    try {
      final player = await _playerService.getPlayer(_player.id);
      if (!mounted) return;
      setState(() => _player = player);
    } on PlayerServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) setState(() => _errorMessage = error.message);
    } on FormatException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } finally {
      if (mounted) setState(() => _isRefreshing = false);
    }

    if (mounted) await _showUnlockDialogIfNeeded();
  }

  Future<void> _showUnlockDialogIfNeeded() async {
    if (_unlockDialogShown ||
        _player.totalExp < widget.university.expRequired ||
        !mounted) {
      return;
    }

    _unlockDialogShown = true;
    await showDialog<void>(
      context: context,
      builder: (context) => TweenAnimationBuilder<double>(
        tween: Tween(begin: 0.5, end: 1),
        duration: const Duration(milliseconds: 300),
        builder: (context, scale, child) =>
            Transform.scale(scale: scale, child: child),
        child: AlertDialog(
          icon: const _SparklingUniversityIcon(),
          title: const Text('Chúc mừng!'),
          content: Text(
            'Bạn đã mở khóa ${widget.university.name} '
            '(${widget.university.shortName})!',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Tiếp tục'),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final targetExp = widget.university.expRequired;
    final progress = targetExp <= 0
        ? 1.0
        : (_player.totalExp / targetExp).clamp(0.0, 1.0);
    final progressPercent = (progress * 100).round();
    final isUniversityUnlocked = _player.totalExp >= targetExp;

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
                  'Xin chào, ${_player.name}!',
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
                const SizedBox(height: 16),
                Text(
                  'Trường mục tiêu: ${widget.university.name} '
                  '(${widget.university.shortName})',
                ),
                const SizedBox(height: 12),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('EXP: ${_player.totalExp} / $targetExp'),
                    Text('$progressPercent%'),
                  ],
                ),
                const SizedBox(height: 8),
                Semantics(
                  label: 'Tiến độ EXP: $progressPercent%',
                  child: LinearProgressIndicator(
                    value: progress,
                    minHeight: 10,
                    color: AppColors.exp,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  isUniversityUnlocked ? 'Đã mở khóa' : 'Chưa mở khóa',
                  style: TextStyle(
                    color: isUniversityUnlocked ? AppColors.success : null,
                    fontWeight: isUniversityUnlocked ? FontWeight.bold : null,
                  ),
                ),
                if (_errorMessage != null) ...[
                  const SizedBox(height: 12),
                  Text(
                    'Không thể cập nhật EXP: $_errorMessage',
                    style: TextStyle(color: AppColors.error),
                  ),
                  TextButton(
                    onPressed: _isRefreshing ? null : _refreshPlayer,
                    child: const Text('Thử cập nhật lại'),
                  ),
                ],
                const SizedBox(height: 20),
                FilledButton(
                  onPressed: _openStagesMap,
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

class _SparklingUniversityIcon extends StatefulWidget {
  const _SparklingUniversityIcon();

  @override
  State<_SparklingUniversityIcon> createState() =>
      _SparklingUniversityIconState();
}

class _SparklingUniversityIconState extends State<_SparklingUniversityIcon>
    with SingleTickerProviderStateMixin {
  late final AnimationController _sparkleController;
  late final Animation<double> _sparkleAnimation;

  @override
  void initState() {
    super.initState();
    _sparkleController = AnimationController(
      duration: const Duration(milliseconds: 900),
      vsync: this,
    )..repeat(reverse: true);
    _sparkleAnimation = CurvedAnimation(
      parent: _sparkleController,
      curve: Curves.easeInOut,
    );
  }

  @override
  void dispose() {
    _sparkleController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 72,
      height: 72,
      child: Stack(
        alignment: Alignment.center,
        children: [
          const Icon(Icons.school_rounded, size: 42, color: AppColors.exp),
          Positioned(top: 4, right: 5, child: _buildSparkle(size: 17)),
          Positioned(bottom: 8, left: 5, child: _buildSparkle(size: 13)),
          Positioned(top: 13, left: 7, child: _buildSparkle(size: 10)),
        ],
      ),
    );
  }

  Widget _buildSparkle({required double size}) {
    return FadeTransition(
      opacity: _sparkleAnimation,
      child: ScaleTransition(
        scale: _sparkleAnimation,
        child: Icon(Icons.auto_awesome, size: size, color: AppColors.exp),
      ),
    );
  }
}
