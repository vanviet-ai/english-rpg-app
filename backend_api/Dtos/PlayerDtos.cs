namespace EnglishRpg.Api.Dtos;

public class PlayerLoginRequest
{
    public string Name { get; set; } = string.Empty;
}

public class SetPlayerTargetRequest
{
    public int UniversityId { get; set; }
}

public class UniversitySummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int ExpRequired { get; set; }
}

public class PlayerResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalExp { get; set; }
    public int? TargetUniversityId { get; set; }
    public UniversitySummaryDto? TargetUniversity { get; set; }
    public bool IsTargetUnlocked { get; set; }
}
