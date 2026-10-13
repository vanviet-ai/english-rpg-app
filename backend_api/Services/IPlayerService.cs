using EnglishRpg.Api.Dtos;

namespace EnglishRpg.Api.Services;

public interface IPlayerService
{
    Task<PlayerResponseDto> LoginAsync(string name, CancellationToken cancellationToken = default);
    Task<PlayerResponseDto> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PlayerResponseDto> SetTargetUniversityAsync(int id, int universityId, CancellationToken cancellationToken = default);
}
