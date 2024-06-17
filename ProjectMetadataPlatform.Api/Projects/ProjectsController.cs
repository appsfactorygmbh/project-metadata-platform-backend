using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Projects;

/// <summary>
///     Endpoints for managing projects.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    /// <summary>
    ///     Creates a new instance of the <see cref="ProjectsController" /> class.
    /// </summary>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Gets all projects or all projects that match the given search string.
    /// </summary>
    /// <param name="search">Search string to filter the projects by.</param>
    /// <returns>All projects or all projects that match the given search string.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> Get(string search = " ")
    {
        var query = new GetAllProjectsQuery(search);
        IEnumerable<Project> projects;
        try
        {
            projects = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        IEnumerable<GetProjectsResponse> response = projects.Select(project => new GetProjectsResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber));

        return Ok(response);
    }

    /// <summary>
    ///     Gets the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The project.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetProjectResponse>> Get(int id)
    {
        var query = new GetProjectQuery(id);
        Project? project;
        try
        {
            project = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (project == null)
        {
            return NotFound(project);
        }

        var response = new GetProjectResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department);

        return Ok(response);
    }
    /// <summary>
    ///     Creates a new project.
    /// </summary>
    /// <param name="project">The data of the new project.</param>
    /// <returns>An response containing the id of the created project.</returns>
    /// <response code="201">The Project has been created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    public async Task<ActionResult<CreateProjectResponse>> Put([FromBody] CreateProjectRequest project)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(project.ProjectName) || string.IsNullOrWhiteSpace(project.BusinessUnit)
                                                               || string.IsNullOrWhiteSpace(project.Department)
                                                               || string.IsNullOrWhiteSpace(project.ClientName))
            {
                return BadRequest("ProjectName, BusinessUnit, Department and ClientName must not be empty.");
            }

            var command = new CreateProjectCommand(project.ProjectName, project.BusinessUnit, project.TeamNumber,
                project.Department, project.ClientName);
            int id = await _mediator.Send(command);

            var response = new CreateProjectResponse(id);
            return Created("/Projects/" + id, response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
