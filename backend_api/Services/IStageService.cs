using EnglishRpg.Api.Dtos;

namespace EnglishRpg.Api.Services;

public interface IStageService
{
    Task<IReadOnlyList<StageResponse>> GetStagesAsync(int playerId, CancellationToken cancellationToken);
    Task<StartStageAttemptResponse> StartAttemptAsync(int stageId, StartStageAttemptRequest request, CancellationToken cancellationToken);
    Task<StartStageAttemptResponse> GetAttemptAsync(int attemptId, CancellationToken cancellationToken);
    Task<AttemptAnswerSubmission> SubmitAnswerAsync(int attemptId, SubmitAttemptAnswerRequest request, CancellationToken cancellationToken);
    Task<NextHintResponse> GetNextHintAsync(int attemptId, NextHintRequest request, CancellationToken cancellationToken);
    Task<AttemptFinishSubmission> FinishAttemptAsync(int attemptId, CancellationToken cancellationToken);
    Task<AttemptSummaryResult> GetAttemptSummaryAsync(int attemptId, CancellationToken cancellationToken);
}
