namespace EnglishRpg.Api.Models;

public class StageAttempt
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int StageId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public StageAttemptStatus Status { get; set; }
    public int CorrectCount { get; set; }
    public int ExpEarned { get; set; }

    public Player Player { get; set; } = null!;
    public Stage Stage { get; set; } = null!;
    public ICollection<AttemptQuestion> AttemptQuestions { get; set; } = new List<AttemptQuestion>();
    public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
