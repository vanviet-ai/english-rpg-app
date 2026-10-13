namespace EnglishRpg.Api.Dtos;

public class UniversityResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int ExpRequired { get; set; }
}
