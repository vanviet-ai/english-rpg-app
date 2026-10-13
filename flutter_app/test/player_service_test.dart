import 'dart:convert';

import 'package:english_rpg_app/models/player.dart';
import 'package:english_rpg_app/models/university.dart';
import 'package:english_rpg_app/services/player_service.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';

void main() {
  test('loads a player by id from the configured API', () async {
    late Uri requestedUri;
    final service = PlayerService(
      client: MockClient((request) async {
        requestedUri = request.url;
        return http.Response.bytes(
          utf8.encode(jsonEncode({'id': 12, 'name': 'Việt', 'totalExp': 1140})),
          200,
        );
      }),
    );

    final player = await service.getPlayer(12);

    expect(requestedUri.toString(), '$apiBaseUrl/players/12');
    expect(player.id, 12);
    expect(player.totalExp, 1140);
    service.dispose();
  });

  test('loads universities from the configured API', () async {
    late Uri requestedUri;
    final service = PlayerService(
      client: MockClient((request) async {
        requestedUri = request.url;
        return http.Response.bytes(
          utf8.encode(
            jsonEncode([
              {
                'id': 1,
                'name': 'ĐH Quốc gia Hà Nội',
                'shortName': 'VNU',
                'expRequired': 10000,
              },
            ]),
          ),
          200,
        );
      }),
    );

    final universities = await service.getUniversities();

    expect(requestedUri.toString(), '$apiBaseUrl/universities');
    expect(universities, hasLength(1));
    expect(universities.single.shortName, 'VNU');
    expect(universities.single.expRequired, 10000);
    service.dispose();
  });

  test(
    'saves the selected target using the player id and university id',
    () async {
      late http.Request requestedRequest;
      final service = PlayerService(
        client: MockClient((request) async {
          requestedRequest = request;
          return http.Response.bytes(
            utf8.encode(jsonEncode({'id': 12, 'name': 'Việt', 'totalExp': 0})),
            200,
          );
        }),
      );
      const player = Player(id: 12, name: 'Việt');
      const university = University(
        id: 3,
        name: 'ĐH Bách khoa Hà Nội',
        shortName: 'HUST',
        expRequired: 9000,
      );

      final updatedPlayer = await service.setTarget(player, university);

      expect(requestedRequest.method, 'PUT');
      expect(requestedRequest.url.toString(), '$apiBaseUrl/players/12/target');
      expect(jsonDecode(requestedRequest.body), {'universityId': 3});
      expect(updatedPlayer.totalExp, 0);
      service.dispose();
    },
  );
}
