namespace EnglishRpg.Api.Models;

public class Stage
{
    public int Id { get; set; }
    public int OrderNo { get; set; }
    public required string Name { get; set; }
    public StageType Type { get; set; }
    public int QuestionCount { get; set; }
    public int PassCorrect { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public int ExpPerCorrect { get; set; }
    public int BonusExp { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<PlayerStageProgress> PlayerProgress { get; set; } = new List<PlayerStageProgress>();
    public ICollection<StageAttempt> Attempts { get; set; } = new List<StageAttempt>();
}
