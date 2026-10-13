import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/player.dart';
import '../models/university.dart';

const apiBaseUrl = 'http://localhost:5137/api';

class PlayerServiceException implements Exception {
  const PlayerServiceException(this.message);

  final String message;
}

class PlayerService {
  PlayerService({http.Client? client}) : _client = client ?? http.Client();

  final http.Client _client;

  void dispose() => _client.close();

  Future<Player> login(String name) async {
    final http.Response response;
    try {
      response = await _client.post(
        Uri.parse('$apiBaseUrl/players/login'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({'name': name}),
      );
    } on http.ClientException {
      throw _connectionException();
    }

    _ensureSuccess(response);

    final body = jsonDecode(response.body);
    if (body is! Map<String, dynamic>) {
      throw const FormatException('Phản hồi đăng nhập không hợp lệ.');
    }
    return Player.fromJson(body);
  }

  Future<Player> getPlayer(int playerId) async {
    final http.Response response;
    try {
      response = await _client.get(Uri.parse('$apiBaseUrl/players/$playerId'));
    } on http.ClientException {
      throw _connectionException();
    }

    _ensureSuccess(response);
    final body = jsonDecode(response.body);
    if (body is! Map<String, dynamic>) {
      throw const FormatException(
        'Phản hồi thông tin người chơi không hợp lệ.',
      );
    }
    return Player.fromJson(body);
  }

  Future<List<University>> getUniversities() async {
    final http.Response response;
    try {
      response = await _client.get(Uri.parse('$apiBaseUrl/universities'));
    } on http.ClientException {
      throw _connectionException();
    }

    _ensureSuccess(response);
    final body = jsonDecode(response.body);
    if (body is! List<dynamic>) {
      throw const FormatException('Danh sách trường không hợp lệ.');
    }
    return body.map((item) {
      if (item is! Map<String, dynamic>) {
        throw const FormatException('Dữ liệu trường đại học không hợp lệ.');
      }
      return University.fromJson(item);
    }).toList();
  }

  Future<Player> setTarget(Player player, University university) async {
    final http.Response response;
    try {
      response = await _client.put(
        Uri.parse('$apiBaseUrl/players/${player.id}/target'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({'universityId': university.id}),
      );
    } on http.ClientException {
      throw _connectionException();
    }

    _ensureSuccess(response);
    final body = jsonDecode(response.body);
    if (body is! Map<String, dynamic>) {
      throw const FormatException('Phản hồi lưu trường mục tiêu không hợp lệ.');
    }
    return Player.fromJson(body);
  }

  void _ensureSuccess(http.Response response) {
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw PlayerServiceException(_messageFromError(response));
    }
  }

  PlayerServiceException _connectionException() {
    return const PlayerServiceException(
      'Không thể kết nối đến máy chủ. Vui lòng kiểm tra backend đang chạy.',
    );
  }

  String _messageFromError(http.Response response) {
    try {
      final body = jsonDecode(response.body);
      if (body is Map<String, dynamic> && body['message'] is String) {
        return body['message'] as String;
      }
    } on FormatException {
      // Non-JSON error responses use the status code below.
    }
    return 'Yêu cầu thất bại (${response.statusCode}). Vui lòng thử lại.';
  }
}
