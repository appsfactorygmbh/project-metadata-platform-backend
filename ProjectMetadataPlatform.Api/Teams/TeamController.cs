using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Application.Teams;

namespace ProjectMetadataPlatform.Api.Teams;

/// <summary>
/// Endpoints for managing teams.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="TeamsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new team with the given attributes.
    /// </summary>
    /// <param name="request">The request body.</param>
    /// <returns>An HTTP Created Response and the Id of the new Plugin.</returns>
    /// <response code="201">The team was created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="409">The team name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateTeamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateTeamResponse>> Put([FromBody] CreateTeamRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TeamName))
        {
            return BadRequest(new ErrorResponse("TeamName can't be empty or whitespaces"));
        }

        var command = new CreateTeamCommand(
            TeamName: request.TeamName,
            BusinessUnit: request.BusinessUnit,
            PTL: request.PTL
        );

        var teamId = await _mediator.Send(command);

        var response = new CreateTeamResponse(teamId);
        var uri = "/Teams/" + teamId;
        return Created(uri, response);
    }

    /// <summary>
    /// Gets the team with the given id.
    /// </summary>
    /// <param name="id">The id of the team.</param>
    /// <returns>The team.</returns>
    /// <response code="200">The team is returned successfully.</response>
    /// <response code="404">The team could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTeamResponse>> Get(int id)
    {
        var query = new GetTeamQuery(id);
        var team = await _mediator.Send(query);

        var response = new GetTeamResponse()
        {
            Id = team.Id,
            TeamName = team.TeamName,
            BusinessUnit = team.BusinessUnit,
            PTL = team.PTL,
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets all teams that match the given filters. Filters are optional.
    /// </summary>
    /// <param name="teamName">Search string to filter teams with that team name.</param>
    /// <param name="search">Search string to filter the teams by (across all attributes).</param>
    /// <returns>All teams that match the given filters.</returns>
    /// <response code="200">The teams are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetTeamResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetTeamResponse>>> Get(
        string? teamName = "",
        string? search = ""
    )
    {
        var query = new GetAllTeamsQuery(FullTextQuery: search, TeamName: teamName);
        var teams = await _mediator.Send(query);
        var response = teams.Select(t => new GetTeamResponse()
        {
            Id = t.Id,
            TeamName = t.TeamName,
            BusinessUnit = t.BusinessUnit,
            PTL = t.PTL,
        });
        return Ok(response);
    }

    /// <summary>
    /// Updates a team.
    /// </summary>
    /// <param name="teamId">The id of the team to update.</param>
    /// <param name="request">The request body containing the details of the team to be updated.</param>
    /// <returns>The updated version of the team.</returns>
    /// <response code="200">The team was updated successfully.</response>
    /// <response code="404">No team with the requested id was found.</response>
    /// <response code="409">The team name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{pluginId:int}")]
    [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetTeamResponse>> Patch(
        int teamId,
        [FromBody] PatchTeamRequest request
    )
    {
        var command = new PatchTeamCommand(
            Id: teamId,
            TeamName: request.TeamName,
            PTL: request.PTL,
            BusinessUnit: request.BusinessUnit
        );

        var team = await _mediator.Send(command);

        var response = new GetTeamResponse()
        {
            Id = team.Id,
            TeamName = team.TeamName,
            BusinessUnit = team.BusinessUnit,
            PTL = team.PTL,
        };
        return Ok(response);
    }
}
