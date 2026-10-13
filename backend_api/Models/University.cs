namespace EnglishRpg.Api.Models;

public class University
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public int ExpRequired { get; set; }

    public ICollection<Player> Players { get; set; } = new List<Player>();
}
