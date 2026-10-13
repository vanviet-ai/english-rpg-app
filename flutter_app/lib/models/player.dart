class Player {
  const Player({required this.id, required this.name, this.totalExp = 0});

  final int id;
  final String name;
  final int totalExp;

  factory Player.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final name = json['name'];
    final totalExp = json['totalExp'] ?? 0;
    if (id is! int || name is! String || totalExp is! int) {
      throw const FormatException('Dữ liệu người chơi không hợp lệ.');
    }

    return Player(id: id, name: name, totalExp: totalExp);
  }
}
