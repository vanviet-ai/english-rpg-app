using EnglishRpg.Api.Data;
using EnglishRpg.Api.Dtos;
using EnglishRpg.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishRpg.Api.Services;

public class PlayerService(AppDbContext dbContext) : IPlayerService
{
    public async Task<PlayerResponseDto> LoginAsync(string name, CancellationToken cancellationToken = default)
    {
        var trimmedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ArgumentException("Player name is required.", nameof(name));
        }

        var player = await dbContext.Players
            .Include(currentPlayer => currentPlayer.TargetUniversity)
            .FirstOrDefaultAsync(currentPlayer => currentPlayer.Name == trimmedName || currentPlayer.Name.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (player is not null)
        {
            return MapPlayer(player);
        }

        var newPlayer = new Player
        {
            Name = trimmedName,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Players.Add(newPlayer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapPlayer(newPlayer);
    }

    public async Task<PlayerResponseDto> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var player = await dbContext.Players
            .Include(currentPlayer => currentPlayer.TargetUniversity)
            .FirstOrDefaultAsync(currentPlayer => currentPlayer.Id == id, cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id {id} was not found.");
        }

        return MapPlayer(player);
    }

    public async Task<PlayerResponseDto> SetTargetUniversityAsync(int id, int universityId, CancellationToken cancellationToken = default)
    {
        if (universityId <= 0)
        {
            throw new ArgumentException("University id must be greater than zero.", nameof(universityId));
        }

        var player = await dbContext.Players
            .Include(currentPlayer => currentPlayer.TargetUniversity)
            .FirstOrDefaultAsync(currentPlayer => currentPlayer.Id == id, cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id {id} was not found.");
        }

        var university = await dbContext.Universities
            .FirstOrDefaultAsync(item => item.Id == universityId, cancellationToken);

        if (university is null)
        {
            throw new KeyNotFoundException($"University with id {universityId} was not found.");
        }

        if (player.TargetUniversityId is not null && player.TargetUniversityId != universityId)
        {
            throw new InvalidOperationException("Target university cannot be changed after it has been selected.");
        }

        if (player.TargetUniversityId is null)
        {
            player.TargetUniversityId = universityId;
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Entry(player).Reference(currentPlayer => currentPlayer.TargetUniversity).LoadAsync(cancellationToken);
        }

        return MapPlayer(player);
    }

    private static PlayerResponseDto MapPlayer(Player player)
    {
        var targetUniversity = player.TargetUniversity;
        var isTargetUnlocked = targetUniversity is not null && player.TotalExp >= targetUniversity.ExpRequired;

        return new PlayerResponseDto
        {
            Id = player.Id,
            Name = player.Name,
            TotalExp = player.TotalExp,
            TargetUniversityId = player.TargetUniversityId,
            TargetUniversity = targetUniversity is null
                ? null
                : new UniversitySummaryDto
                {
                    Id = targetUniversity.Id,
                    Name = targetUniversity.Name,
                    ShortName = targetUniversity.ShortName,
                    ExpRequired = targetUniversity.ExpRequired
                },
            IsTargetUnlocked = isTargetUnlocked
        };
    }
}
