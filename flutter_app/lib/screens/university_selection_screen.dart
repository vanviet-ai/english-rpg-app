import 'package:flutter/material.dart';

import '../models/player.dart';
import '../models/university.dart';
import '../services/player_service.dart';
import 'home_screen.dart';

class UniversitySelectionScreen extends StatefulWidget {
  const UniversitySelectionScreen({super.key, required this.player});

  final Player player;

  @override
  State<UniversitySelectionScreen> createState() =>
      _UniversitySelectionScreenState();
}

class _UniversitySelectionScreenState extends State<UniversitySelectionScreen> {
  final _playerService = PlayerService();
  List<University>? _universities;
  String? _errorMessage;
  int? _savingUniversityId;

  @override
  void initState() {
    super.initState();
    _loadUniversities();
  }

  @override
  void dispose() {
    _playerService.dispose();
    super.dispose();
  }

  Future<void> _loadUniversities() async {
    setState(() => _errorMessage = null);
    try {
      final universities = await _playerService.getUniversities();
      if (mounted) setState(() => _universities = universities);
    } on PlayerServiceException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } on FormatException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    }
  }

  Future<void> _selectUniversity(University university) async {
    setState(() {
      _savingUniversityId = university.id;
      _errorMessage = null;
    });

    try {
      final player = await _playerService.setTarget(widget.player, university);
      if (!mounted) return;
      await Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) => HomeScreen(player: player, university: university),
        ),
      );
    } on PlayerServiceException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } on FormatException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } finally {
      if (mounted) setState(() => _savingUniversityId = null);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Chọn trường')),
      body: _universities == null
          ? _buildLoadingOrError()
          : ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: _universities!.length + 1,
              separatorBuilder: (_, _) => const SizedBox(height: 8),
              itemBuilder: (context, index) {
                if (index == 0) {
                  return Padding(
                    padding: const EdgeInsets.only(bottom: 8),
                    child: Text(
                      'Xin chào, ${widget.player.name}! Hãy chọn trường mục tiêu.',
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  );
                }
                final university = _universities![index - 1];
                final isSaving = _savingUniversityId == university.id;
                return Card(
                  child: ListTile(
                    enabled: _savingUniversityId == null,
                    title: Text(university.name),
                    subtitle: Text(
                      '${university.shortName} · EXP mục tiêu: ${university.expRequired}',
                    ),
                    trailing: isSaving
                        ? const SizedBox(
                            width: 22,
                            height: 22,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(Icons.chevron_right),
                    onTap: () => _selectUniversity(university),
                  ),
                );
              },
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
            FilledButton(
              onPressed: _loadUniversities,
              child: const Text('Thử lại'),
            ),
          ],
        ),
      ),
    );
  }
}
