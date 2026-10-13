using EnglishRpg.Api.Data;
using EnglishRpg.Api.Dtos;
using EnglishRpg.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishRpg.Api.Services;

public sealed class StageService(AppDbContext dbContext) : IStageService
{
    private static readonly TimeSpan NormalAttemptDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan BossAttemptDuration = TimeSpan.FromMinutes(30);
    private const int BossQuestionCount = 15;
    private const int NextHintExpCost = 60;

    public async Task<IReadOnlyList<StageResponse>> GetStagesAsync(int playerId, CancellationToken cancellationToken)
    {
        var playerExists = await dbContext.Players.AnyAsync(player => player.Id == playerId, cancellationToken);
        if (!playerExists)
        {
            throw new KeyNotFoundException($"Player with id {playerId} was not found.");
        }

        var stages = await dbContext.Stages
            .OrderBy(stage => stage.OrderNo)
            .ToListAsync(cancellationToken);
        var progress = await dbContext.PlayerStageProgress
            .Where(item => item.PlayerId == playerId)
            .ToDictionaryAsync(item => item.StageId, cancellationToken);
        var now = DateTime.UtcNow;
        var inProgressStageIds = await dbContext.StageAttempts
            .Where(attempt => attempt.PlayerId == playerId &&
                              attempt.Status == StageAttemptStatus.InProgress &&
                              attempt.ExpiresAt > now &&
                              attempt.Stage.Type == StageType.Normal)
            .Select(attempt => attempt.StageId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var inProgressSet = inProgressStageIds.ToHashSet();
        var completedNormalOrders = stages
            .Where(stage => stage.Type == StageType.Normal &&
                            progress.TryGetValue(stage.Id, out var item) &&
                            item.IsCompleted)
            .Select(stage => stage.OrderNo)
            .ToHashSet();

        return stages.Select(stage =>
        {
            var isCompleted = progress.TryGetValue(stage.Id, out var stageProgress) &&
                              stageProgress.IsCompleted;
            var isAvailable = stage.Type == StageType.Boss
                ? completedNormalOrders.Count >= 3
                : stage.OrderNo == 1 || completedNormalOrders.Contains(stage.OrderNo - 1);

            return new StageResponse(
                stage.Id,
                stage.OrderNo,
                stage.Name,
                stage.Type.ToString(),
                isCompleted ? "completed" : isAvailable ? "open" : "locked",
                inProgressSet.Contains(stage.Id));
        }).ToList();
    }

    public async Task<StartStageAttemptResponse> StartAttemptAsync(
        int stageId,
        StartStageAttemptRequest request,
        CancellationToken cancellationToken)
    {
        var player = await dbContext.Players
            .FirstOrDefaultAsync(item => item.Id == request.PlayerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Player with id {request.PlayerId} was not found.");
        var stage = await dbContext.Stages
            .FirstOrDefaultAsync(item => item.Id == stageId, cancellationToken)
            ?? throw new KeyNotFoundException($"Stage with id {stageId} was not found.");

        if (stage.Type == StageType.Boss)
        {
            return await StartBossAttemptAsync(player, stage, cancellationToken);
        }

        if (stage.Type != StageType.Normal)
        {
            throw new InvalidOperationException("This stage type cannot be started.");
        }

        var previousStageCompleted = stage.OrderNo == 1 ||
            await dbContext.PlayerStageProgress.AnyAsync(
                item => item.PlayerId == request.PlayerId &&
                        item.Stage.OrderNo == stage.OrderNo - 1 &&
                        item.Stage.Type == StageType.Normal &&
                        item.IsCompleted,
                cancellationToken);
        if (!previousStageCompleted)
        {
            throw new InvalidOperationException("This stage is locked until the previous normal stage is passed.");
        }

        var now = DateTime.UtcNow;
        var activeAttempt = await dbContext.StageAttempts
            .Include(attempt => attempt.AttemptQuestions)
                .ThenInclude(attemptQuestion => attemptQuestion.Question)
            .Include(attempt => attempt.AttemptAnswers)
            .Where(attempt => attempt.PlayerId == request.PlayerId &&
                              attempt.StageId == stageId &&
                              attempt.Status == StageAttemptStatus.InProgress)
            .OrderByDescending(attempt => attempt.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeAttempt is not null && activeAttempt.ExpiresAt <= now)
        {
            activeAttempt.Status = StageAttemptStatus.Expired;
            await dbContext.SaveChangesAsync(cancellationToken);
            activeAttempt = null;
        }

        if (activeAttempt is not null && !request.Restart)
        {
            return MapAttempt(activeAttempt);
        }

        if (activeAttempt is not null)
        {
            activeAttempt.Status = StageAttemptStatus.Expired;
        }

        var questionPool = await dbContext.Questions
            .Where(question => question.StageId == stageId)
            .ToListAsync(cancellationToken);
        if (questionPool.Count < stage.QuestionCount)
        {
            throw new InvalidOperationException(
                $"Stage {stageId} requires {stage.QuestionCount} questions, but only {questionPool.Count} are available.");
        }

        var selectedQuestions = questionPool
            .OrderBy(question => question.OrderNo)
            .Take(stage.QuestionCount)
            .ToList();
        var attempt = new StageAttempt
        {
            PlayerId = player.Id,
            StageId = stage.Id,
            StartedAt = now,
            ExpiresAt = now.Add(NormalAttemptDuration),
            Status = StageAttemptStatus.InProgress,
            AttemptQuestions = selectedQuestions.Select((question, index) => new AttemptQuestion
            {
                QuestionId = question.Id,
                OrderNo = index + 1
            }).ToList()
        };

        dbContext.StageAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(attempt)
            .Collection(item => item.AttemptQuestions)
            .Query()
            .Include(item => item.Question)
            .LoadAsync(cancellationToken);

        return MapAttempt(attempt);
    }

    private async Task<StartStageAttemptResponse> StartBossAttemptAsync(
        Player player,
        Stage stage,
        CancellationToken cancellationToken)
    {
        var completedNormalStages = await dbContext.PlayerStageProgress
            .CountAsync(
                item => item.PlayerId == player.Id &&
                        item.IsCompleted &&
                        item.Stage.Type == StageType.Normal,
                cancellationToken);
        if (completedNormalStages < 3)
        {
            throw new InvalidOperationException("The Boss is locked until all three normal stages are completed.");
        }

        var questionPool = await dbContext.Questions
            .Where(question => question.StageId == stage.Id)
            .ToListAsync(cancellationToken);
        if (questionPool.Count < BossQuestionCount)
        {
            throw new InvalidOperationException(
                $"The Boss requires {BossQuestionCount} questions, but only {questionPool.Count} are available.");
        }

        var previousAttempts = await dbContext.StageAttempts
            .Where(attempt => attempt.PlayerId == player.Id &&
                              attempt.StageId == stage.Id &&
                              attempt.Status == StageAttemptStatus.InProgress)
            .ToListAsync(cancellationToken);
        foreach (var previousAttempt in previousAttempts)
        {
            previousAttempt.Status = StageAttemptStatus.Expired;
        }

        Shuffle(questionPool);
        var now = DateTime.UtcNow;
        var attempt = new StageAttempt
        {
            PlayerId = player.Id,
            StageId = stage.Id,
            StartedAt = now,
            ExpiresAt = now.Add(BossAttemptDuration),
            Status = StageAttemptStatus.InProgress,
            AttemptQuestions = questionPool
                .Take(BossQuestionCount)
                .Select((question, index) => new AttemptQuestion
                {
                    QuestionId = question.Id,
                    OrderNo = index + 1
                })
                .ToList()
        };

        dbContext.StageAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(attempt)
            .Collection(item => item.AttemptQuestions)
            .Query()
            .Include(item => item.Question)
            .LoadAsync(cancellationToken);

        return MapAttempt(attempt);
    }

    public async Task<StartStageAttemptResponse> GetAttemptAsync(
        int attemptId,
        CancellationToken cancellationToken)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, cancellationToken);
        if (attempt.Stage.Type != StageType.Normal)
        {
            throw new InvalidOperationException("Boss attempts cannot be resumed.");
        }

        await dbContext.Entry(attempt)
            .Collection(item => item.AttemptAnswers)
            .LoadAsync(cancellationToken);
        return MapAttempt(attempt);
    }

    public async Task<AttemptAnswerSubmission> SubmitAnswerAsync(
        int attemptId,
        SubmitAttemptAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, cancellationToken);
        var attemptQuestion = attempt.AttemptQuestions
            .FirstOrDefault(item => item.QuestionId == request.QuestionId)
            ?? throw new ArgumentException("Question does not belong to this attempt.", nameof(request.QuestionId));

        var alreadyAnswered = await dbContext.AttemptAnswers.AnyAsync(
            answer => answer.AttemptId == attemptId && answer.QuestionId == request.QuestionId,
            cancellationToken);
        if (alreadyAnswered)
        {
            throw new InvalidOperationException("This question has already been answered.");
        }

        var selectedOption = request.SelectedOption.Trim().ToUpperInvariant();
        var isCorrect = selectedOption == attemptQuestion.Question.CorrectOption;
        var isBoss = attempt.Stage.Type == StageType.Boss;
        var answer = new AttemptAnswer
        {
            AttemptId = attempt.Id,
            QuestionId = request.QuestionId,
            SelectedOption = selectedOption,
            IsCorrect = isCorrect,
            ShownHint1 = !isBoss && !isCorrect,
            ExpDelta = !isBoss && isCorrect ? attempt.Stage.ExpPerCorrect : 0
        };
        attempt.AttemptAnswers.Add(answer);
        if (isCorrect)
        {
            attempt.CorrectCount++;
        }

        if (!isBoss)
        {
            attempt.ExpiresAt = DateTime.UtcNow.Add(NormalAttemptDuration);
        }

        attempt.ExpEarned = CalculateAttemptExp(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);

        return isBoss
            ? new AttemptAnswerSubmission(
                null,
                new BossAnswerAcknowledgementResponse(
                    request.QuestionId,
                    true,
                    ToUtcOffset(attempt.ExpiresAt)))
            : new AttemptAnswerSubmission(
                new SubmitAttemptAnswerResponse(
                    request.QuestionId,
                    isCorrect,
                    isCorrect ? null : attemptQuestion.Question.Hint1,
                    ToUtcOffset(attempt.ExpiresAt)),
                null);
    }

    public async Task<NextHintResponse> GetNextHintAsync(
        int attemptId,
        NextHintRequest request,
        CancellationToken cancellationToken)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, cancellationToken);
        if (attempt.Stage.Type == StageType.Boss)
        {
            throw new InvalidOperationException("Hints are not available in Boss attempts.");
        }

        var attemptQuestion = attempt.AttemptQuestions
            .FirstOrDefault(item => item.QuestionId == request.QuestionId)
            ?? throw new ArgumentException("Question does not belong to this attempt.", nameof(request.QuestionId));
        var answer = await dbContext.AttemptAnswers
            .FirstOrDefaultAsync(
                item => item.AttemptId == attemptId && item.QuestionId == request.QuestionId,
                cancellationToken)
            ?? throw new InvalidOperationException("Answer the question incorrectly before requesting the next hint.");

        if (answer.IsCorrect)
        {
            throw new InvalidOperationException("The next hint is only available after an incorrect answer.");
        }

        if (answer.UsedNextHint)
        {
            throw new InvalidOperationException("The next hint has already been used for this question.");
        }

        if (string.IsNullOrWhiteSpace(attemptQuestion.Question.Hint2))
        {
            throw new InvalidOperationException("The second hint is not configured for this question.");
        }

        answer.UsedNextHint = true;
        answer.ExpDelta -= NextHintExpCost;
        attempt.ExpiresAt = DateTime.UtcNow.Add(NormalAttemptDuration);
        attempt.ExpEarned = CalculateAttemptExp(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new NextHintResponse(
            request.QuestionId,
            attemptQuestion.Question.Hint2,
            NextHintExpCost,
            attempt.ExpEarned,
            ToUtcOffset(attempt.ExpiresAt));
    }

    public async Task<AttemptFinishSubmission> FinishAttemptAsync(
        int attemptId,
        CancellationToken cancellationToken)
    {
        var attempt = await dbContext.StageAttempts
            .Include(item => item.Stage)
            .Include(item => item.AttemptQuestions)
                .ThenInclude(item => item.Question)
            .Include(item => item.AttemptAnswers)
                .ThenInclude(item => item.Question)
            .FirstOrDefaultAsync(item => item.Id == attemptId, cancellationToken)
            ?? throw new KeyNotFoundException($"Attempt with id {attemptId} was not found.");

        EnsureAttemptInProgress(attempt);
        var now = DateTime.UtcNow;
        var isBoss = attempt.Stage.Type == StageType.Boss;
        var timedOut = attempt.ExpiresAt <= now;
        if (timedOut && !isBoss)
        {
            attempt.Status = StageAttemptStatus.Expired;
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("This attempt has expired.");
        }

        if (!isBoss && attempt.AttemptAnswers.Count != attempt.AttemptQuestions.Count)
        {
            throw new InvalidOperationException("All questions must be answered before the attempt can be finished.");
        }

        attempt.CorrectCount = attempt.AttemptAnswers.Count(answer => answer.IsCorrect);
        var passed = !timedOut && attempt.CorrectCount >= attempt.Stage.PassCorrect;
        attempt.Status = passed ? StageAttemptStatus.Passed : StageAttemptStatus.Failed;
        attempt.FinishedAt = now;

        var progress = await dbContext.PlayerStageProgress
            .FirstOrDefaultAsync(
                item => item.PlayerId == attempt.PlayerId && item.StageId == attempt.StageId,
                cancellationToken);
        if (progress is null)
        {
            progress = new PlayerStageProgress
            {
                PlayerId = attempt.PlayerId,
                StageId = attempt.StageId
            };
            dbContext.PlayerStageProgress.Add(progress);
        }

        if (passed)
        {
            progress.IsCompleted = true;
            progress.FirstClearedAt ??= now;
        }

        if (isBoss)
        {
            attempt.ExpEarned = passed && !progress.BossRewardClaimed
                ? attempt.Stage.BonusExp
                : 0;
            if (passed)
            {
                progress.BossRewardClaimed = true;
            }
        }
        else
        {
            progress.BestCorrectCount = Math.Max(progress.BestCorrectCount, attempt.CorrectCount);
            attempt.ExpEarned = CalculateAttemptExp(attempt);
        }

        var player = await dbContext.Players
            .FirstAsync(item => item.Id == attempt.PlayerId, cancellationToken);
        player.TotalExp += attempt.ExpEarned;
        await dbContext.SaveChangesAsync(cancellationToken);

        if (isBoss)
        {
            return new AttemptFinishSubmission(
                null,
                new BossFinishAttemptResponse(
                    attempt.Id,
                    attempt.StageId,
                    attempt.Status.ToString(),
                    attempt.CorrectCount,
                    attempt.AttemptQuestions.Count - attempt.CorrectCount,
                    attempt.ExpEarned,
                    player.TotalExp));
        }

        var incorrectQuestions = attempt.AttemptQuestions
            .Where(item => attempt.AttemptAnswers.Any(answer =>
                answer.QuestionId == item.QuestionId && !answer.IsCorrect))
            .OrderBy(item => item.OrderNo)
            .Select(item =>
            {
                var answer = attempt.AttemptAnswers.Single(value => value.QuestionId == item.QuestionId);
                return new IncorrectQuestionReviewResponse(
                    item.QuestionId,
                    item.OrderNo,
                    item.Question.Content,
                    answer.SelectedOption!,
                    item.Question.CorrectOption,
                    item.Question.Hint1,
                    item.Question.Hint2,
                    item.Question.Explanation);
            })
            .ToList();

        return new AttemptFinishSubmission(
            new FinishStageAttemptResponse(
                attempt.Id,
                attempt.StageId,
                attempt.Status.ToString(),
                attempt.CorrectCount,
                attempt.AttemptQuestions.Count - attempt.CorrectCount,
                attempt.ExpEarned,
                player.TotalExp,
                incorrectQuestions),
            null);
    }

    public async Task<AttemptSummaryResult> GetAttemptSummaryAsync(
        int attemptId,
        CancellationToken cancellationToken)
    {
        var attempt = await dbContext.StageAttempts
            .Include(item => item.Stage)
            .Include(item => item.AttemptQuestions)
                .ThenInclude(item => item.Question)
            .Include(item => item.AttemptAnswers)
            .FirstOrDefaultAsync(item => item.Id == attemptId, cancellationToken)
            ?? throw new KeyNotFoundException($"Attempt with id {attemptId} was not found.");

        if (attempt.Status is StageAttemptStatus.InProgress or StageAttemptStatus.Expired)
        {
            throw new InvalidOperationException("The attempt must be finished before its summary is available.");
        }

        if (attempt.Stage.Type == StageType.Boss)
        {
            var answers = attempt.AttemptAnswers.ToDictionary(answer => answer.QuestionId);
            var questions = attempt.AttemptQuestions
                .OrderBy(item => item.OrderNo)
                .Select(item =>
                {
                    answers.TryGetValue(item.QuestionId, out var answer);
                    return new BossQuestionReviewResponse(
                        item.QuestionId,
                        item.OrderNo,
                        item.Question.Content,
                        answer?.SelectedOption,
                        item.Question.CorrectOption,
                        item.Question.Explanation);
                })
                .ToList();

            return new AttemptSummaryResult(
                null,
                new BossAttemptSummaryResponse(
                    attempt.Id,
                    attempt.StageId,
                    attempt.Status.ToString(),
                    attempt.CorrectCount,
                    attempt.AttemptQuestions.Count - attempt.CorrectCount,
                    questions));
        }

        var incorrectQuestions = attempt.AttemptQuestions
            .Where(item => attempt.AttemptAnswers.Any(answer =>
                answer.QuestionId == item.QuestionId && !answer.IsCorrect))
            .OrderBy(item => item.OrderNo)
            .Select(item =>
            {
                var answer = attempt.AttemptAnswers.Single(value => value.QuestionId == item.QuestionId);
                return new IncorrectQuestionReviewResponse(
                    item.QuestionId,
                    item.OrderNo,
                    item.Question.Content,
                    answer.SelectedOption!,
                    item.Question.CorrectOption,
                    item.Question.Hint1,
                    item.Question.Hint2,
                    item.Question.Explanation);
            })
            .ToList();

        return new AttemptSummaryResult(
            new NormalAttemptSummaryResponse(
                attempt.Id,
                attempt.StageId,
                attempt.Status.ToString(),
                incorrectQuestions),
            null);
    }

    private async Task<StageAttempt> GetActiveAttemptAsync(int attemptId, CancellationToken cancellationToken)
    {
        var attempt = await dbContext.StageAttempts
            .Include(item => item.Stage)
            .Include(item => item.AttemptQuestions)
                .ThenInclude(item => item.Question)
            .Include(item => item.AttemptAnswers)
            .FirstOrDefaultAsync(item => item.Id == attemptId, cancellationToken)
            ?? throw new KeyNotFoundException($"Attempt with id {attemptId} was not found.");

        EnsureAttemptInProgress(attempt);
        if (attempt.ExpiresAt <= DateTime.UtcNow)
        {
            if (attempt.Stage.Type == StageType.Normal)
            {
                attempt.Status = StageAttemptStatus.Expired;
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            throw new InvalidOperationException("This attempt has expired.");
        }

        return attempt;
    }

    private static void EnsureAttemptInProgress(StageAttempt attempt)
    {
        if (attempt.Status != StageAttemptStatus.InProgress)
        {
            throw new InvalidOperationException("This attempt is no longer in progress.");
        }
    }

    private static int CalculateAttemptExp(StageAttempt attempt)
    {
        var correctCount = attempt.AttemptAnswers.Count(answer => answer.IsCorrect);
        var hintCost = attempt.AttemptAnswers.Sum(answer => answer.UsedNextHint ? NextHintExpCost : 0);
        return Math.Max(0, correctCount * attempt.Stage.ExpPerCorrect - hintCost);
    }

    private static void Shuffle<T>(IList<T> items)
    {
        for (var index = items.Count - 1; index > 0; index--)
        {
            var otherIndex = Random.Shared.Next(index + 1);
            (items[index], items[otherIndex]) = (items[otherIndex], items[index]);
        }
    }

    private static StartStageAttemptResponse MapAttempt(StageAttempt attempt)
    {
        var answersByQuestion = attempt.AttemptAnswers.ToDictionary(answer => answer.QuestionId);
        var questions = attempt.AttemptQuestions
            .OrderBy(item => item.OrderNo)
            .Select(item =>
            {
                answersByQuestion.TryGetValue(item.QuestionId, out var answer);
                return new AttemptQuestionResponse(
                    item.QuestionId,
                    item.OrderNo,
                    item.Question.Content,
                    item.Question.OptionA,
                    item.Question.OptionB,
                    item.Question.OptionC,
                    item.Question.OptionD,
                    answer is not null,
                    answer?.SelectedOption);
            })
            .ToList();

        return new StartStageAttemptResponse(
            attempt.Id,
            attempt.StageId,
            ToUtcOffset(attempt.ExpiresAt),
            questions);
    }

    private static DateTimeOffset ToUtcOffset(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));
}
