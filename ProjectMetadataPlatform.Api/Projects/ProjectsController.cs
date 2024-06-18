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
    /// Retrieves all projects or projects with search pattern.
    /// </summary>
    /// <param name="search">Search pattern to look for in ProjectName</param>
    /// <returns>All projects. When search is used all Projects, which are fitting in pattern</returns>
    /// <response code="200">Projects are returned successfully</response>
    /// <response code="500">Internal Server Error</response>
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

        var response = projects.Select(project => new GetProjectsResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber));

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a project by id.
    /// </summary>
    /// <param name="id">Identifiacation number for the project</param>
    /// <returns>A project.</returns>
    /// <response code="200">Project is returned successfully</response>
    /// <response code="404">Project not found</response>
    /// <response code="500">Internal Server Error</response>
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
    /// Creates a new project or replaces an existing project.
    /// </summary>
    /// <param name="project">New Project that has to be added.</param>
    /// <returns>Id of the created project</returns>
    /// <response code="201">Project is created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPut]
    public async Task<ActionResult<Project>> Put([FromBody] CreateProjectRequest project)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(project.ProjectName) || string.IsNullOrWhiteSpace(project.BusinessUnit) || string.IsNullOrWhiteSpace(project.Department) || string.IsNullOrWhiteSpace(project.ClientName))
            {
                return BadRequest("ProjectName, BusinessUnit, Department and ClientName must not be empty.");
            }

            var command = new CreateProjectCommand(project.ProjectName, project.BusinessUnit, project.TeamNumber,
                project.Department, project.ClientName);
           var id = await _mediator.Send(command);
            
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