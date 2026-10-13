using System.ComponentModel.DataAnnotations;

namespace EnglishRpg.Api.Dtos;

/// <summary>Describes a stage and its availability for a player.</summary>
public sealed record StageResponse(
    int Id,
    int OrderNo,
    string Name,
    string Type,
    string Status,
    bool HasInProgressAttempt);

/// <summary>Request to start or restart a normal-stage attempt.</summary>
public sealed record StartStageAttemptRequest
{
    [Range(1, int.MaxValue)]
    public required int PlayerId { get; init; }

    public bool Restart { get; init; }
}

/// <summary>A question and its options without answer or hint information.</summary>
public sealed record AttemptQuestionResponse(
    int QuestionId,
    int OrderNo,
    string Content,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    bool IsAnswered,
    string? SelectedOption);

/// <summary>Details of a newly created or resumed attempt.</summary>
public sealed record StartStageAttemptResponse(
    int AttemptId,
    int StageId,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<AttemptQuestionResponse> Questions);

public sealed record AttemptAnswerSubmission(
    SubmitAttemptAnswerResponse? NormalResponse,
    BossAnswerAcknowledgementResponse? BossResponse);

/// <summary>Request to answer a question in an attempt.</summary>
public sealed record SubmitAttemptAnswerRequest
{
    [Range(1, int.MaxValue)]
    public required int QuestionId { get; init; }

    [Required, RegularExpression("^[A-Da-d]$")]
    public required string SelectedOption { get; init; }
}

/// <summary>Result of answering a question; an incorrect answer includes the free first hint.</summary>
public sealed record SubmitAttemptAnswerResponse(
    int QuestionId,
    bool IsCorrect,
    string? Hint1,
    DateTimeOffset ExpiresAt);

public sealed record BossAnswerAcknowledgementResponse(
    int QuestionId,
    bool Recorded,
    DateTimeOffset ExpiresAt);

/// <summary>Request to reveal the second hint for an incorrectly answered question.</summary>
public sealed record NextHintRequest
{
    [Range(1, int.MaxValue)]
    public required int QuestionId { get; init; }
}

/// <summary>The second hint and its effect on the attempt's EXP.</summary>
public sealed record NextHintResponse(
    int QuestionId,
    string? Hint2,
    int ExpDeducted,
    int AttemptExp,
    DateTimeOffset ExpiresAt);

/// <summary>A question review shown after an attempt is finished.</summary>
public sealed record IncorrectQuestionReviewResponse(
    int QuestionId,
    int OrderNo,
    string Content,
    string SelectedOption,
    string CorrectOption,
    string? Hint1,
    string? Hint2,
    string? Explanation);

/// <summary>Final score and EXP awarded for a finished stage attempt.</summary>
public sealed record FinishStageAttemptResponse(
    int AttemptId,
    int StageId,
    string Status,
    int CorrectCount,
    int IncorrectCount,
    int ExpEarned,
    int TotalExp,
    IReadOnlyList<IncorrectQuestionReviewResponse> IncorrectQuestions);

public sealed record BossFinishAttemptResponse(
    int AttemptId,
    int StageId,
    string Status,
    int CorrectCount,
    int IncorrectCount,
    int ExpEarned,
    int TotalExp);

public sealed record AttemptFinishSubmission(
    FinishStageAttemptResponse? NormalResponse,
    BossFinishAttemptResponse? BossResponse);

public sealed record NormalAttemptSummaryResponse(
    int AttemptId,
    int StageId,
    string Status,
    IReadOnlyList<IncorrectQuestionReviewResponse> IncorrectQuestions);

public sealed record BossQuestionReviewResponse(
    int QuestionId,
    int OrderNo,
    string Content,
    string? SelectedOption,
    string CorrectOption,
    string? Explanation);

public sealed record BossAttemptSummaryResponse(
    int AttemptId,
    int StageId,
    string Status,
    int CorrectCount,
    int IncorrectCount,
    IReadOnlyList<BossQuestionReviewResponse> Questions);

public sealed record AttemptSummaryResult(
    NormalAttemptSummaryResponse? NormalResponse,
    BossAttemptSummaryResponse? BossResponse);
