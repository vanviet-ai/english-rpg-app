import 'package:english_rpg_app/widgets/app_dialogs.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('resume dialog distinguishes continuing from restarting', (
    tester,
  ) async {
    Future<bool?>? dialogResult;
    await tester.pumpWidget(
      MaterialApp(
        home: Builder(
          builder: (context) => Scaffold(
            body: TextButton(
              onPressed: () => dialogResult = showResumeStageDialog(context),
              child: const Text('Open dialog'),
            ),
          ),
        ),
      ),
    );

    await tester.tap(find.text('Open dialog'));
    await tester.pumpAndSettle();

    expect(find.text('Tiếp tục lượt dở?'), findsOneWidget);
    expect(find.text('Tiếp tục'), findsOneWidget);
    expect(find.text('Chơi lại'), findsOneWidget);

    await tester.tap(find.text('Tiếp tục'));
    await tester.pumpAndSettle();
    expect(await dialogResult, isFalse);

    await tester.tap(find.text('Open dialog'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Chơi lại'));
    await tester.pumpAndSettle();
    expect(await dialogResult, isTrue);
  });

  testWidgets(
    'network dialog shows the connection error and can be dismissed',
    (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: Builder(
            builder: (context) => Scaffold(
              body: TextButton(
                onPressed: () => showNetworkErrorDialog(
                  context,
                  'Không thể kết nối đến máy chủ.',
                ),
                child: const Text('Open dialog'),
              ),
            ),
          ),
        ),
      );

      await tester.tap(find.text('Open dialog'));
      await tester.pumpAndSettle();

      expect(find.text('Mất kết nối'), findsOneWidget);
      expect(find.text('Không thể kết nối đến máy chủ.'), findsOneWidget);
      await tester.tap(find.text('Đóng'));
      await tester.pumpAndSettle();
      expect(find.byType(AlertDialog), findsNothing);
    },
  );
}
