class University {
  const University({
    required this.id,
    required this.name,
    required this.shortName,
    required this.expRequired,
  });

  final int id;
  final String name;
  final String shortName;
  final int expRequired;

  factory University.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final name = json['name'];
    final shortName = json['shortName'];
    final expRequired = json['expRequired'];
    if (id is! int ||
        name is! String ||
        shortName is! String ||
        expRequired is! int) {
      throw const FormatException('Dữ liệu trường đại học không hợp lệ.');
    }

    return University(
      id: id,
      name: name,
      shortName: shortName,
      expRequired: expRequired,
    );
  }
}
