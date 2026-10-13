namespace EnglishRpg.Api.Models;

public class Question
{
    public int Id { get; set; }
    public int StageId { get; set; }
    public int OrderNo { get; set; }
    public required string Content { get; set; }
    public required string OptionA { get; set; }
    public required string OptionB { get; set; }
    public required string OptionC { get; set; }
    public required string OptionD { get; set; }
    public required string CorrectOption { get; set; }
    public string? Hint1 { get; set; }
    public string? Hint2 { get; set; }
    public string? Explanation { get; set; }

    public Stage Stage { get; set; } = null!;
    public ICollection<AttemptQuestion> AttemptQuestions { get; set; } = new List<AttemptQuestion>();
    public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
