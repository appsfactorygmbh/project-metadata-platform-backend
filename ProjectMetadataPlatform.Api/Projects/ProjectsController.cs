using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Api.Projects;

/// <summary>
/// Controller for projects.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    /// <summary>
    /// Creates a new instance of the <see cref="ProjectsController"/> class.
    /// </summary>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>All projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> Get()
    {
        var query = new GetAllProjectsQuery();

        var projects = await _mediator.Send(query);

        var response = projects.Select(project => new GetProjectsResponse(
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber));

        return Ok(response);
    }
}