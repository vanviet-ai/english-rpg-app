using EnglishRpg.Api.Dtos;
using EnglishRpg.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnglishRpg.Api.Controllers;

/// <summary>Stage listing and ordinary-stage attempt operations.</summary>
[ApiController]
[Route("api")]
public sealed class StagesController(IStageService stageService) : ControllerBase
{
    /// <summary>Returns stage availability and active-attempt state for a player.</summary>
    [HttpGet("stages")]
    [ProducesResponseType<IReadOnlyList<StageResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<StageResponse>>> GetStages(
        [FromQuery] int playerId,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await stageService.GetStagesAsync(playerId, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Player not found", detail: exception.Message);
        }
    }

    /// <summary>Starts a new ordinary-stage attempt or resumes the current one.</summary>
    [HttpPost("stages/{stageId:int}/start")]
    [ProducesResponseType<StartStageAttemptResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StartStageAttemptResponse>> Start(
        int stageId,
        [FromBody] StartStageAttemptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await stageService.StartAttemptAsync(stageId, request, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Player or stage not found", detail: exception.Message);
        }
        catch (ArgumentException exception)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid request", detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Stage cannot be started", detail: exception.Message);
        }
    }

    /// <summary>Submits a one-time answer and returns the free first hint when incorrect.</summary>
    [HttpPost("attempts/{attemptId:int}/answer")]
    [ProducesResponseType<SubmitAttemptAnswerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmitAttemptAnswerResponse>> Answer(
        int attemptId,
        [FromBody] SubmitAttemptAnswerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await stageService.SubmitAnswerAsync(attemptId, request, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Attempt not found", detail: exception.Message);
        }
        catch (ArgumentException exception)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid answer", detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Answer cannot be submitted", detail: exception.Message);
        }
    }

    /// <summary>Reveals the paid second hint for an incorrectly answered question.</summary>
    [HttpPost("attempts/{attemptId:int}/hint/next")]
    [ProducesResponseType<NextHintResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<NextHintResponse>> NextHint(
        int attemptId,
        [FromBody] NextHintRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await stageService.GetNextHintAsync(attemptId, request, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Attempt not found", detail: exception.Message);
        }
        catch (ArgumentException exception)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid question", detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Hint cannot be requested", detail: exception.Message);
        }
    }

    /// <summary>Finishes a fully answered attempt and awards its calculated EXP.</summary>
    [HttpPost("attempts/{attemptId:int}/finish")]
    [ProducesResponseType<FinishStageAttemptResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FinishStageAttemptResponse>> Finish(
        int attemptId,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await stageService.FinishAttemptAsync(attemptId, cancellationToken));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Attempt not found", detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Attempt cannot be finished", detail: exception.Message);
        }
    }
}
