using EnglishRpg.Api.Dtos;
using EnglishRpg.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnglishRpg.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController(IPlayerService playerService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<PlayerResponseDto>> Login([FromBody] PlayerLoginRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        try
        {
            var player = await playerService.LoginAsync(request.Name, cancellationToken);
            return Ok(player);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var player = await playerService.GetPlayerByIdAsync(id, cancellationToken);
            return Ok(player);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/target")]
    public async Task<ActionResult<PlayerResponseDto>> SetTarget(int id, [FromBody] SetPlayerTargetRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        try
        {
            var player = await playerService.SetTargetUniversityAsync(id, request.UniversityId, cancellationToken);
            return Ok(player);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
