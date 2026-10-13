namespace EnglishRpg.Api.Models;

public class AttemptAnswer
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public string? SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
    public bool ShownHint1 { get; set; }
    public bool UsedNextHint { get; set; }
    public int ExpDelta { get; set; }

    public StageAttempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
