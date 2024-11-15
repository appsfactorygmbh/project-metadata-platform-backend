using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Logs;

[ApiController]
// [Authorize]
[Route("[controller]")]
public class LogsController: ControllerBase
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{projectId:int}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetLogResponse>>> Get(int? projectId)
    {
        var query = new GetLogsQuery(projectId);

        IEnumerable<LogResponse> logs;

        try
        {
            logs = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return Ok(logs);
    }
}
