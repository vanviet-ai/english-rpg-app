import 'dart:convert';

import 'package:english_rpg_app/services/player_service.dart';
import 'package:english_rpg_app/services/stage_service.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

void main() {
  test('uses Boss endpoints and derives its timer from server time', () async {
    final requests = <http.Request>[];
    final service = StageService(
      client: MockClient((request) async {
        requests.add(request);
        if (request.url.path.endsWith('/start')) {
          return http.Response(
            jsonEncode({
              'attemptId': 91,
              'stageId': 4,
              'expiresAt': '2026-10-14T18:40:00Z',
              'questions': [
                {
                  'questionId': 42,
                  'orderNo': 1,
                  'content': 'Choose the correct answer.',
                  'optionA': 'A',
                  'optionB': 'B',
                  'optionC': 'C',
                  'optionD': 'D',
                  'isAnswered': false,
                  'selectedOption': null,
                },
              ],
            }),
            200,
            headers: {'date': 'Wed, 14 Oct 2026 18:10:00 GMT'},
          );
        }
        if (request.url.path.endsWith('/answer')) {
          return http.Response(
            jsonEncode({'questionId': 42, 'recorded': true}),
            200,
          );
        }
        if (request.url.path.endsWith('/finish')) {
          return http.Response(
            jsonEncode({
              'attemptId': 91,
              'stageId': 4,
              'status': 'Passed',
              'correctCount': 13,
              'incorrectCount': 2,
              'expEarned': 2000,
              'totalExp': 2000,
            }),
            200,
          );
        }
        return http.Response(
          jsonEncode({
            'attemptId': 91,
            'stageId': 4,
            'status': 'Passed',
            'correctCount': 13,
            'incorrectCount': 2,
            'questions': [
              {
                'questionId': 42,
                'orderNo': 1,
                'content': 'Choose the correct answer.',
                'selectedOption': 'B',
                'correctOption': 'A',
                'explanation': 'A is correct.',
              },
            ],
          }),
          200,
        );
      }),
    );

    final attempt = await service.startBossStage(playerId: 7);
    await service.submitBossAnswer(
      attempt.attemptId,
      questionId: 42,
      selectedOption: 'B',
    );
    final result = await service.finishBoss(attempt.attemptId);
    final summary = await service.getBossSummary(attempt.attemptId);

    expect(requests[0].url.toString(), '$apiBaseUrl/stages/4/start');
    expect(jsonDecode(requests[0].body), {'playerId': 7, 'restart': false});
    expect(
      attempt.remainingAtResponse,
      greaterThan(const Duration(minutes: 29)),
    );
    expect(requests[1].url.toString(), '$apiBaseUrl/attempts/91/answer');
    expect(jsonDecode(requests[1].body), {
      'questionId': 42,
      'selectedOption': 'B',
    });
    expect(requests[2].url.toString(), '$apiBaseUrl/attempts/91/finish');
    expect(result.isVictory, isTrue);
    expect(result.expEarned, 2000);
    expect(requests[3].url.toString(), '$apiBaseUrl/attempts/91/summary');
    expect(summary.questions.single.correctOption, 'A');
    service.dispose();
  });

  test('uses the stage API endpoints and parses attempt results', () async {
    final requests = <http.Request>[];
    final service = StageService(
      client: MockClient((request) async {
        requests.add(request);
        if (request.method == 'GET') {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode([
                {
                  'id': 1,
                  'orderNo': 1,
                  'name': 'Ải 1',
                  'type': 'Normal',
                  'status': 'open',
                  'hasInProgressAttempt': false,
                },
              ]),
            ),
            200,
          );
        }
        if (request.url.path.endsWith('/start')) {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode({
                'attemptId': 9,
                'stageId': 1,
                'expiresAt': '2026-10-13T00:00:00Z',
                'questions': [
                  {
                    'questionId': 42,
                    'orderNo': 1,
                    'content': 'Choose the correct answer.',
                    'optionA': 'A',
                    'optionB': 'B',
                    'optionC': 'C',
                    'optionD': 'D',
                    'isAnswered': false,
                    'selectedOption': null,
                  },
                ],
              }),
            ),
            200,
          );
        }
        if (request.url.path.endsWith('/answer')) {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode({
                'questionId': 42,
                'isCorrect': false,
                'hint1': 'Hint 1',
              }),
            ),
            200,
          );
        }
        if (request.url.path.endsWith('/hint/next')) {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode({
                'questionId': 42,
                'hint2': 'Hint 2',
                'expDeducted': 60,
                'attemptExp': 0,
              }),
            ),
            200,
          );
        }
        return http.Response.bytes(
          utf8.encode(
            jsonEncode({
              'attemptId': 9,
              'stageId': 1,
              'status': 'Failed',
              'correctCount': 0,
              'incorrectCount': 1,
              'expEarned': 0,
              'totalExp': 0,
              'incorrectQuestions': [
                {
                  'questionId': 42,
                  'orderNo': 1,
                  'content': 'Choose the correct answer.',
                  'selectedOption': 'B',
                  'correctOption': 'A',
                  'hint1': 'Hint 1',
                  'hint2': 'Hint 2',
                  'explanation': null,
                },
              ],
            }),
          ),
          200,
        );
      }),
    );

    final stages = await service.getStages(7);
    final attempt = await service.startStage(1, playerId: 7, restart: false);
    final answer = await service.submitAnswer(
      attempt.attemptId,
      questionId: 42,
      selectedOption: 'B',
    );
    final hint = await service.getNextHint(attempt.attemptId, questionId: 42);
    final summary = await service.finishStage(attempt.attemptId);

    expect(requests[0].url.toString(), '$apiBaseUrl/stages?playerId=7');
    expect(requests[1].url.toString(), '$apiBaseUrl/stages/1/start');
    expect(jsonDecode(requests[1].body), {'playerId': 7, 'restart': false});
    expect(requests[2].url.toString(), '$apiBaseUrl/attempts/9/answer');
    expect(jsonDecode(requests[2].body), {
      'questionId': 42,
      'selectedOption': 'B',
    });
    expect(requests[3].url.toString(), '$apiBaseUrl/attempts/9/hint/next');
    expect(jsonDecode(requests[3].body), {'questionId': 42});
    expect(requests[4].url.toString(), '$apiBaseUrl/attempts/9/finish');
    expect(stages.single.status, 'open');
    expect(attempt.questions.single.content, 'Choose the correct answer.');
    expect(answer.isCorrect, isFalse);
    expect(answer.hint1, 'Hint 1');
    expect(hint.hint2, 'Hint 2');
    expect(hint.expDeducted, 60);
    expect(summary.incorrectCount, 1);
    expect(summary.incorrectQuestions.single.correctOption, 'A');
    service.dispose();
  });
}
