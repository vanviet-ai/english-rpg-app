using EnglishRpg.Api.Dtos;
using EnglishRpg.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnglishRpg.Api.Controllers;

[ApiController]
[Route("api")]
public class UniversitiesController(IUniversityService universityService) : ControllerBase
{
    [HttpGet("universities")]
    public async Task<ActionResult<List<UniversityResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var universities = await universityService.GetAllAsync(cancellationToken);
        return Ok(universities);
    }
}
