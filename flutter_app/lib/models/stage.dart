class Stage {
  const Stage({
    required this.id,
    required this.orderNo,
    required this.name,
    required this.type,
    required this.status,
    required this.hasInProgressAttempt,
  });

  final int id;
  final int orderNo;
  final String name;
  final String type;
  final String status;
  final bool hasInProgressAttempt;

  bool get isLocked => status == 'locked';
  bool get isNormal => type.toLowerCase() == 'normal';

  factory Stage.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final orderNo = json['orderNo'];
    final name = json['name'];
    final type = json['type'];
    final status = json['status'];
    final hasInProgressAttempt = json['hasInProgressAttempt'];
    if (id is! int ||
        orderNo is! int ||
        name is! String ||
        type is! String ||
        status is! String ||
        hasInProgressAttempt is! bool) {
      throw const FormatException('Dữ liệu ải không hợp lệ.');
    }

    return Stage(
      id: id,
      orderNo: orderNo,
      name: name,
      type: type,
      status: status,
      hasInProgressAttempt: hasInProgressAttempt,
    );
  }
}

class AttemptQuestion {
  const AttemptQuestion({
    required this.questionId,
    required this.orderNo,
    required this.content,
    required this.optionA,
    required this.optionB,
    required this.optionC,
    required this.optionD,
    required this.isAnswered,
    required this.selectedOption,
  });

  final int questionId;
  final int orderNo;
  final String content;
  final String optionA;
  final String optionB;
  final String optionC;
  final String optionD;
  final bool isAnswered;
  final String? selectedOption;

  List<String> get options => [optionA, optionB, optionC, optionD];

  factory AttemptQuestion.fromJson(Map<String, dynamic> json) {
    final questionId = json['questionId'];
    final orderNo = json['orderNo'];
    final content = json['content'];
    final optionA = json['optionA'];
    final optionB = json['optionB'];
    final optionC = json['optionC'];
    final optionD = json['optionD'];
    final isAnswered = json['isAnswered'];
    final selectedOption = json['selectedOption'];
    if (questionId is! int ||
        orderNo is! int ||
        content is! String ||
        optionA is! String ||
        optionB is! String ||
        optionC is! String ||
        optionD is! String ||
        isAnswered is! bool ||
        (selectedOption != null && selectedOption is! String)) {
      throw const FormatException('Dữ liệu câu hỏi không hợp lệ.');
    }

    return AttemptQuestion(
      questionId: questionId,
      orderNo: orderNo,
      content: content,
      optionA: optionA,
      optionB: optionB,
      optionC: optionC,
      optionD: optionD,
      isAnswered: isAnswered,
      selectedOption: selectedOption as String?,
    );
  }
}

class StageAttempt {
  const StageAttempt({
    required this.attemptId,
    required this.stageId,
    required this.expiresAt,
    this.remainingAtResponse,
    required this.questions,
  });

  final int attemptId;
  final int stageId;
  final DateTime expiresAt;
  final Duration? remainingAtResponse;
  final List<AttemptQuestion> questions;

  factory StageAttempt.fromJson(
    Map<String, dynamic> json, {
    Duration? remainingAtResponse,
  }) {
    final attemptId = json['attemptId'];
    final stageId = json['stageId'];
    final expiresAt = json['expiresAt'];
    final questions = json['questions'];
    if (attemptId is! int ||
        stageId is! int ||
        expiresAt is! String ||
        questions is! List<dynamic>) {
      throw const FormatException('Dữ liệu lượt chơi không hợp lệ.');
    }
    final parsedExpiresAt = DateTime.tryParse(expiresAt);
    if (parsedExpiresAt == null) {
      throw const FormatException('Thời hạn lượt chơi không hợp lệ.');
    }

    return StageAttempt(
      attemptId: attemptId,
      stageId: stageId,
      expiresAt: parsedExpiresAt.toUtc(),
      remainingAtResponse: remainingAtResponse,
      questions: questions.map((item) {
        if (item is! Map<String, dynamic>) {
          throw const FormatException('Dữ liệu câu hỏi không hợp lệ.');
        }
        return AttemptQuestion.fromJson(item);
      }).toList(),
    );
  }
}

class BossFinishResult {
  const BossFinishResult({
    required this.attemptId,
    required this.stageId,
    required this.status,
    required this.correctCount,
    required this.incorrectCount,
    required this.expEarned,
    required this.totalExp,
  });

  final int attemptId;
  final int stageId;
  final String status;
  final int correctCount;
  final int incorrectCount;
  final int expEarned;
  final int totalExp;

  bool get isVictory => status.toLowerCase() == 'passed';

  factory BossFinishResult.fromJson(Map<String, dynamic> json) {
    final attemptId = json['attemptId'];
    final stageId = json['stageId'];
    final status = json['status'];
    final correctCount = json['correctCount'];
    final incorrectCount = json['incorrectCount'];
    final expEarned = json['expEarned'];
    final totalExp = json['totalExp'];
    if (attemptId is! int ||
        stageId is! int ||
        status is! String ||
        correctCount is! int ||
        incorrectCount is! int ||
        expEarned is! int ||
        totalExp is! int) {
      throw const FormatException('Dữ liệu kết quả Boss không hợp lệ.');
    }
    return BossFinishResult(
      attemptId: attemptId,
      stageId: stageId,
      status: status,
      correctCount: correctCount,
      incorrectCount: incorrectCount,
      expEarned: expEarned,
      totalExp: totalExp,
    );
  }
}

class BossQuestionReview {
  const BossQuestionReview({
    required this.questionId,
    required this.orderNo,
    required this.content,
    required this.selectedOption,
    required this.correctOption,
    required this.explanation,
  });

  final int questionId;
  final int orderNo;
  final String content;
  final String? selectedOption;
  final String correctOption;
  final String? explanation;

  factory BossQuestionReview.fromJson(Map<String, dynamic> json) {
    final questionId = json['questionId'];
    final orderNo = json['orderNo'];
    final content = json['content'];
    final selectedOption = json['selectedOption'];
    final correctOption = json['correctOption'];
    final explanation = json['explanation'];
    if (questionId is! int ||
        orderNo is! int ||
        content is! String ||
        (selectedOption != null && selectedOption is! String) ||
        correctOption is! String ||
        (explanation != null && explanation is! String)) {
      throw const FormatException('Dữ liệu ôn tập Boss không hợp lệ.');
    }
    return BossQuestionReview(
      questionId: questionId,
      orderNo: orderNo,
      content: content,
      selectedOption: selectedOption as String?,
      correctOption: correctOption,
      explanation: explanation as String?,
    );
  }
}

class BossAttemptSummary {
  const BossAttemptSummary({
    required this.attemptId,
    required this.stageId,
    required this.status,
    required this.correctCount,
    required this.incorrectCount,
    required this.questions,
  });

  final int attemptId;
  final int stageId;
  final String status;
  final int correctCount;
  final int incorrectCount;
  final List<BossQuestionReview> questions;

  factory BossAttemptSummary.fromJson(Map<String, dynamic> json) {
    final attemptId = json['attemptId'];
    final stageId = json['stageId'];
    final status = json['status'];
    final correctCount = json['correctCount'];
    final incorrectCount = json['incorrectCount'];
    final questions = json['questions'];
    if (attemptId is! int ||
        stageId is! int ||
        status is! String ||
        correctCount is! int ||
        incorrectCount is! int ||
        questions is! List<dynamic>) {
      throw const FormatException('Dữ liệu tổng kết Boss không hợp lệ.');
    }
    return BossAttemptSummary(
      attemptId: attemptId,
      stageId: stageId,
      status: status,
      correctCount: correctCount,
      incorrectCount: incorrectCount,
      questions: questions.map((item) {
        if (item is! Map<String, dynamic>) {
          throw const FormatException('Dữ liệu ôn tập Boss không hợp lệ.');
        }
        return BossQuestionReview.fromJson(item);
      }).toList(),
    );
  }
}

class AnswerResult {
  const AnswerResult({required this.isCorrect, required this.hint1});

  final bool isCorrect;
  final String? hint1;

  factory AnswerResult.fromJson(Map<String, dynamic> json) {
    final isCorrect = json['isCorrect'];
    final hint1 = json['hint1'];
    if (isCorrect is! bool || (hint1 != null && hint1 is! String)) {
      throw const FormatException('Kết quả trả lời không hợp lệ.');
    }
    return AnswerResult(isCorrect: isCorrect, hint1: hint1 as String?);
  }
}

class NextHint {
  const NextHint({required this.hint2, required this.expDeducted});

  final String? hint2;
  final int expDeducted;

  factory NextHint.fromJson(Map<String, dynamic> json) {
    final hint2 = json['hint2'];
    final expDeducted = json['expDeducted'];
    if ((hint2 != null && hint2 is! String) || expDeducted is! int) {
      throw const FormatException('Dữ liệu gợi ý không hợp lệ.');
    }
    return NextHint(hint2: hint2 as String?, expDeducted: expDeducted);
  }
}

class IncorrectQuestionReview {
  const IncorrectQuestionReview({
    required this.orderNo,
    required this.content,
    required this.selectedOption,
    required this.correctOption,
    required this.hint1,
    required this.hint2,
    required this.explanation,
  });

  final int orderNo;
  final String content;
  final String selectedOption;
  final String correctOption;
  final String? hint1;
  final String? hint2;
  final String? explanation;

  factory IncorrectQuestionReview.fromJson(Map<String, dynamic> json) {
    final orderNo = json['orderNo'];
    final content = json['content'];
    final selectedOption = json['selectedOption'];
    final correctOption = json['correctOption'];
    final hint1 = json['hint1'];
    final hint2 = json['hint2'];
    final explanation = json['explanation'];
    if (orderNo is! int ||
        content is! String ||
        selectedOption is! String ||
        correctOption is! String ||
        (hint1 != null && hint1 is! String) ||
        (hint2 != null && hint2 is! String) ||
        (explanation != null && explanation is! String)) {
      throw const FormatException('Dữ liệu ôn tập câu sai không hợp lệ.');
    }
    return IncorrectQuestionReview(
      orderNo: orderNo,
      content: content,
      selectedOption: selectedOption,
      correctOption: correctOption,
      hint1: hint1 as String?,
      hint2: hint2 as String?,
      explanation: explanation as String?,
    );
  }
}

class StageSummary {
  const StageSummary({
    required this.correctCount,
    required this.incorrectCount,
    required this.expEarned,
    required this.incorrectQuestions,
  });

  final int correctCount;
  final int incorrectCount;
  final int expEarned;
  final List<IncorrectQuestionReview> incorrectQuestions;

  factory StageSummary.fromJson(Map<String, dynamic> json) {
    final correctCount = json['correctCount'];
    final incorrectCount = json['incorrectCount'];
    final expEarned = json['expEarned'];
    final incorrectQuestions = json['incorrectQuestions'];
    if (correctCount is! int ||
        incorrectCount is! int ||
        expEarned is! int ||
        incorrectQuestions is! List<dynamic>) {
      throw const FormatException('Dữ liệu tổng kết không hợp lệ.');
    }
    return StageSummary(
      correctCount: correctCount,
      incorrectCount: incorrectCount,
      expEarned: expEarned,
      incorrectQuestions: incorrectQuestions.map((item) {
        if (item is! Map<String, dynamic>) {
          throw const FormatException('Dữ liệu ôn tập câu sai không hợp lệ.');
        }
        return IncorrectQuestionReview.fromJson(item);
      }).toList(),
    );
  }
}
