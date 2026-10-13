namespace EnglishRpg.Api.Models;

public class AttemptQuestion
{
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public int OrderNo { get; set; }

    public StageAttempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
