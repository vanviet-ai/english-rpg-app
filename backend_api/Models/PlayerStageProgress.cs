namespace EnglishRpg.Api.Models;

public class PlayerStageProgress
{
    public int PlayerId { get; set; }
    public int StageId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? FirstClearedAt { get; set; }
    public int BestCorrectCount { get; set; }
    public bool BossRewardClaimed { get; set; }

    public Player Player { get; set; } = null!;
    public Stage Stage { get; set; } = null!;
}
