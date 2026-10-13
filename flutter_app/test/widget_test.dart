import 'package:flutter_test/flutter_test.dart';

import 'package:english_rpg_app/main.dart';

void main() {
  testWidgets('shows player name input and validates empty submission', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const EnglishRpgApp());

    expect(find.text('Tên người chơi'), findsOneWidget);
    await tester.tap(find.text('Bắt đầu'));
    await tester.pump();

    expect(find.text('Vui lòng nhập tên người chơi.'), findsOneWidget);
  });
}
