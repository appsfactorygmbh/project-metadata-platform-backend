using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Application.Logs;

namespace ProjectMetadataPlatform.Api.Logs;

/// <summary>
/// API controller for managing log entries.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class LogsController: ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves log entries based on the specified project ID and search criteria.
    /// </summary>
    /// <param name="projectId">The ID of the project to filter logs by.</param>
    /// <param name="search">The search term to filter logs by.</param>
    /// <returns>A list of log responses.</returns>
    /// <response code="200">Returns the list of log responses.</response>
    /// <response code="404">Not Project with the given id was found.</response>
    /// <response code="500">If an error occurs while processing the request.</response>
    [HttpGet("{projectId:int}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogResponse>>> Get(int? projectId, string? search)
    {
        var query = new GetLogsQuery(projectId, search);

        IEnumerable<LogResponse> logs;

        try
        {
            logs = await _mediator.Send(query);
        }
        catch (InvalidOperationException)
        {
            return NotFound("No project with id " + projectId + " found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return Ok(logs);
    }
}
