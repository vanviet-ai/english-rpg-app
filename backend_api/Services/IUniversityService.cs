using EnglishRpg.Api.Dtos;

namespace EnglishRpg.Api.Services;

public interface IUniversityService
{
    Task<List<UniversityResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
