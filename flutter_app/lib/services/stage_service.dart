import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/stage.dart';
import 'player_service.dart';

class StageServiceException implements Exception {
  const StageServiceException(this.message);

  final String message;
}

class StageService {
  StageService({http.Client? client}) : _client = client ?? http.Client();

  final http.Client _client;

  void dispose() => _client.close();

  Future<List<Stage>> getStages(int playerId) async {
    final response = await _request(
      () => _client.get(Uri.parse('$apiBaseUrl/stages?playerId=$playerId')),
    );
    _ensureSuccess(response);
    final body = jsonDecode(response.body);
    if (body is! List<dynamic>) {
      throw const FormatException('Danh sách ải không hợp lệ.');
    }
    return body.map((item) {
      if (item is! Map<String, dynamic>) {
        throw const FormatException('Dữ liệu ải không hợp lệ.');
      }
      return Stage.fromJson(item);
    }).toList();
  }

  Future<StageAttempt> startStage(
    int stageId, {
    required int playerId,
    required bool restart,
  }) async {
    final response = await _request(
      () => _client.post(
        Uri.parse('$apiBaseUrl/stages/$stageId/start'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({'playerId': playerId, 'restart': restart}),
      ),
    );
    _ensureSuccess(response);
    return StageAttempt.fromJson(_decodeObject(response));
  }

  Future<StageAttempt> startBossStage({required int playerId}) async {
    final requestTimer = Stopwatch()..start();
    final response = await _request(
      () => _client.post(
        Uri.parse('$apiBaseUrl/stages/4/start'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({'playerId': playerId, 'restart': false}),
      ),
    );
    requestTimer.stop();
    _ensureSuccess(response);
    final attempt = StageAttempt.fromJson(_decodeObject(response));
    final serverTime = _parseHttpDate(response.headers['date']);
    final remaining =
        attempt.expiresAt.difference(serverTime) -
        requestTimer.elapsed -
        const Duration(seconds: 1);
    return StageAttempt(
      attemptId: attempt.attemptId,
      stageId: attempt.stageId,
      expiresAt: attempt.expiresAt,
      remainingAtResponse: remaining.isNegative ? Duration.zero : remaining,
      questions: attempt.questions,
    );
  }

  Future<AnswerResult> submitAnswer(
    int attemptId, {
    required int questionId,
    required String selectedOption,
  }) async {
    final response = await _request(
      () => _client.post(
        Uri.parse('$apiBaseUrl/attempts/$attemptId/answer'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({
          'questionId': questionId,
          'selectedOption': selectedOption,
        }),
      ),
    );
    _ensureSuccess(response);
    return AnswerResult.fromJson(_decodeObject(response));
  }

  Future<NextHint> getNextHint(int attemptId, {required int questionId}) async {
    final response = await _request(
      () => _client.post(
        Uri.parse('$apiBaseUrl/attempts/$attemptId/hint/next'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({'questionId': questionId}),
      ),
    );
    _ensureSuccess(response);
    return NextHint.fromJson(_decodeObject(response));
  }

  Future<StageSummary> finishStage(int attemptId) async {
    final response = await _request(
      () => _client.post(Uri.parse('$apiBaseUrl/attempts/$attemptId/finish')),
    );
    _ensureSuccess(response);
    return StageSummary.fromJson(_decodeObject(response));
  }

  Future<void> submitBossAnswer(
    int attemptId, {
    required int questionId,
    required String selectedOption,
  }) async {
    final response = await _request(
      () => _client.post(
        Uri.parse('$apiBaseUrl/attempts/$attemptId/answer'),
        headers: const {'Content-Type': 'application/json'},
        body: jsonEncode({
          'questionId': questionId,
          'selectedOption': selectedOption,
        }),
      ),
    );
    _ensureSuccess(response);
    final body = _decodeObject(response);
    if (body['questionId'] != questionId || body['recorded'] != true) {
      throw const FormatException(
        'Máy chủ không xác nhận đã ghi nhận câu trả lời Boss.',
      );
    }
  }

  Future<BossFinishResult> finishBoss(int attemptId) async {
    final response = await _request(
      () => _client.post(Uri.parse('$apiBaseUrl/attempts/$attemptId/finish')),
    );
    _ensureSuccess(response);
    return BossFinishResult.fromJson(_decodeObject(response));
  }

  Future<BossAttemptSummary> getBossSummary(int attemptId) async {
    final response = await _request(
      () => _client.get(Uri.parse('$apiBaseUrl/attempts/$attemptId/summary')),
    );
    _ensureSuccess(response);
    return BossAttemptSummary.fromJson(_decodeObject(response));
  }

  Future<http.Response> _request(Future<http.Response> Function() send) async {
    try {
      return await send();
    } on http.ClientException {
      throw const StageServiceException(
        'Không thể kết nối đến máy chủ. Vui lòng kiểm tra backend đang chạy.',
      );
    }
  }

  Map<String, dynamic> _decodeObject(http.Response response) {
    final body = jsonDecode(response.body);
    if (body is! Map<String, dynamic>) {
      throw const FormatException('Phản hồi từ máy chủ không hợp lệ.');
    }
    return body;
  }

  void _ensureSuccess(http.Response response) {
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw StageServiceException(_messageFromError(response));
    }
  }

  String _messageFromError(http.Response response) {
    try {
      final body = jsonDecode(response.body);
      if (body is Map<String, dynamic>) {
        final detail = body['detail'];
        final message = body['message'];
        final title = body['title'];
        if (detail is String) return detail;
        if (message is String) return message;
        if (title is String) return title;
      }
    } on FormatException {
      // Non-JSON error responses use the status code below.
    }
    return 'Yêu cầu thất bại (${response.statusCode}). Vui lòng thử lại.';
  }

  DateTime _parseHttpDate(String? value) {
    if (value == null) {
      throw const FormatException(
        'Không nhận được giờ máy chủ để đồng bộ đồng hồ Boss.',
      );
    }
    final match = RegExp(
      r'^[A-Za-z]{3}, (\d{2}) ([A-Za-z]{3}) (\d{4}) (\d{2}):(\d{2}):(\d{2}) GMT$',
    ).firstMatch(value);
    if (match == null) {
      throw const FormatException('Giờ máy chủ trả về không hợp lệ.');
    }
    const monthNumbers = {
      'Jan': 1,
      'Feb': 2,
      'Mar': 3,
      'Apr': 4,
      'May': 5,
      'Jun': 6,
      'Jul': 7,
      'Aug': 8,
      'Sep': 9,
      'Oct': 10,
      'Nov': 11,
      'Dec': 12,
    };
    final month = monthNumbers[match[2]];
    if (month == null) {
      throw const FormatException('Tháng trong giờ máy chủ không hợp lệ.');
    }
    return DateTime.utc(
      int.parse(match[3]!),
      month,
      int.parse(match[1]!),
      int.parse(match[4]!),
      int.parse(match[5]!),
      int.parse(match[6]!),
    );
  }
}
