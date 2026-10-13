import 'package:flutter/material.dart';

Future<bool?> showResumeStageDialog(BuildContext context) {
  return showDialog<bool>(
    context: context,
    builder: (context) => AlertDialog(
      title: const Text('Tiếp tục lượt dở?'),
      content: const Text(
        'Bạn có một lượt chơi chưa hoàn thành. Bạn muốn tiếp tục hay chơi lại?',
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(true),
          child: const Text('Chơi lại'),
        ),
        FilledButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: const Text('Tiếp tục'),
        ),
      ],
    ),
  );
}

Future<void> showNetworkErrorDialog(BuildContext context, String message) {
  return showDialog<void>(
    context: context,
    builder: (context) => AlertDialog(
      icon: const Icon(Icons.wifi_off_outlined),
      title: const Text('Mất kết nối'),
      content: Text(message),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Đóng'),
        ),
      ],
    ),
  );
}
