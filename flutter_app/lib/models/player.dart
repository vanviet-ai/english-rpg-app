import 'university.dart';

class Player {
  const Player({
    required this.id,
    required this.name,
    this.totalExp = 0,
    this.targetUniversityId,
    this.targetUniversity,
  });

  final int id;
  final String name;
  final int totalExp;
  final int? targetUniversityId;
  final University? targetUniversity;

  factory Player.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final name = json['name'];
    final totalExp = json['totalExp'] ?? 0;
    final rawTargetUniversityId = json['targetUniversityId'];
    final targetUniversityJson = json['targetUniversity'];
    if (id is! int ||
        name is! String ||
        totalExp is! int ||
        (rawTargetUniversityId != null && rawTargetUniversityId is! int) ||
        (targetUniversityJson != null &&
            targetUniversityJson is! Map<String, dynamic>)) {
      throw const FormatException('Dữ liệu người chơi không hợp lệ.');
    }

    final targetUniversityId = rawTargetUniversityId is int
        ? rawTargetUniversityId
        : null;
    final targetUniversity = targetUniversityJson == null
        ? null
        : University.fromJson(targetUniversityJson);
    if (targetUniversityId != null &&
        targetUniversity?.id != targetUniversityId) {
      throw const FormatException(
        'Trường mục tiêu trong dữ liệu người chơi không hợp lệ.',
      );
    }

    return Player(
      id: id,
      name: name,
      totalExp: totalExp,
      targetUniversityId: targetUniversityId,
      targetUniversity: targetUniversity,
    );
  }
}
