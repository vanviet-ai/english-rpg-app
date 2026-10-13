import 'dart:convert';

import 'package:english_rpg_app/screens/login_screen.dart';
import 'package:english_rpg_app/services/player_service.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

void main() {
  testWidgets('opens HomeScreen for a player with a saved target', (
    tester,
  ) async {
    final service = PlayerService(
      client: MockClient((request) async {
        expect(request.url.path, '/api/players/login');
        return http.Response.bytes(
          utf8.encode(
            jsonEncode({
              'id': 12,
              'name': 'Saved target',
              'totalExp': 1200,
              'targetUniversityId': 3,
              'targetUniversity': {
                'id': 3,
                'name': 'ĐH Bách khoa Hà Nội',
                'shortName': 'HUST',
                'expRequired': 9000,
              },
              'isTargetUnlocked': false,
            }),
          ),
          200,
        );
      }),
    );

    await tester.pumpWidget(
      MaterialApp(home: LoginScreen(playerService: service)),
    );
    await tester.enterText(find.byType(TextField), 'Saved target');
    await tester.tap(find.text('Bắt đầu'));
    await tester.pumpAndSettle();

    expect(find.text('Trang chủ'), findsOneWidget);
    expect(find.text('Xin chào, Saved target!'), findsOneWidget);
  });

  testWidgets('opens university selection for a player without a target', (
    tester,
  ) async {
    final service = PlayerService(
      client: MockClient((request) async {
        if (request.url.path == '/api/players/login') {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode({
                'id': 13,
                'name': 'New player',
                'totalExp': 0,
                'targetUniversityId': null,
                'targetUniversity': null,
                'isTargetUnlocked': false,
              }),
            ),
            200,
          );
        }
        if (request.url.path == '/api/universities') {
          return http.Response.bytes(
            utf8.encode(
              jsonEncode([
                {
                  'id': 3,
                  'name': 'ĐH Bách khoa Hà Nội',
                  'shortName': 'HUST',
                  'expRequired': 9000,
                },
              ]),
            ),
            200,
          );
        }
        throw StateError('Unexpected request: ${request.url}');
      }),
    );

    await tester.pumpWidget(
      MaterialApp(home: LoginScreen(playerService: service)),
    );
    await tester.enterText(find.byType(TextField), 'New player');
    await tester.tap(find.text('Bắt đầu'));
    await tester.pumpAndSettle();

    expect(find.text('Chọn trường'), findsOneWidget);
  });
}
