import 'package:flutter/material.dart';

import '../models/player.dart';
import '../services/player_service.dart';
import '../theme/app_theme.dart';
import '../widgets/app_dialogs.dart';
import '../widgets/primary_button.dart';
import 'home_screen.dart';
import 'university_selection_screen.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key, this.playerService});

  final PlayerService? playerService;

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _nameController = TextEditingController();
  late final PlayerService _playerService;
  String? _errorMessage;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _playerService = widget.playerService ?? PlayerService();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _playerService.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    final name = _nameController.text.trim();
    if (name.isEmpty) {
      setState(() => _errorMessage = 'Vui lòng nhập tên người chơi.');
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final Player player = await _playerService.login(name);
      if (!mounted) return;
      final targetUniversityId = player.targetUniversityId;
      final targetUniversity = player.targetUniversity;
      if (targetUniversityId != null &&
          targetUniversity?.id != targetUniversityId) {
        throw const FormatException(
          'Không thể tải trường mục tiêu của người chơi.',
        );
      }
      await Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) => targetUniversityId == null
              ? UniversitySelectionScreen(player: player)
              : HomeScreen(player: player, university: targetUniversity!),
        ),
      );
    } on PlayerServiceException catch (error) {
      if (mounted && error.isConnectionError) {
        await showNetworkErrorDialog(context, error.message);
      }
      if (mounted) setState(() => _errorMessage = error.message);
    } on FormatException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.auto_stories, size: 64, color: AppColors.primary),
                const SizedBox(height: 24),
                Text(
                  'English RPG',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
                const SizedBox(height: 8),
                Text(
                  'Nhập tên để bắt đầu hành trình',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                const SizedBox(height: 32),
                TextField(
                  controller: _nameController,
                  enabled: !_isLoading,
                  textInputAction: TextInputAction.go,
                  onSubmitted: (_) => _login(),
                  decoration: const InputDecoration(
                    labelText: 'Tên người chơi',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.person_outline),
                  ),
                ),
                if (_errorMessage != null) ...[
                  const SizedBox(height: 12),
                  Text(
                    _errorMessage!,
                    style: TextStyle(
                      color: Theme.of(context).colorScheme.error,
                    ),
                    textAlign: TextAlign.center,
                  ),
                ],
                const SizedBox(height: 20),
                PrimaryButton(
                  label: 'Bắt đầu',
                  isLoading: _isLoading,
                  onPressed: _login,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
