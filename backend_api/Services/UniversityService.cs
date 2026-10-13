using EnglishRpg.Api.Data;
using EnglishRpg.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EnglishRpg.Api.Services;

public class UniversityService(AppDbContext dbContext) : IUniversityService
{
    public async Task<List<UniversityResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Universities
            .AsNoTracking()
            .OrderBy(university => university.ExpRequired)
            .Select(university => new UniversityResponseDto
            {
                Id = university.Id,
                Name = university.Name,
                ShortName = university.ShortName,
                ExpRequired = university.ExpRequired
            })
            .ToListAsync(cancellationToken);
    }
}
