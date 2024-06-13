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
    [HttpPut]
    public async Task<ActionResult<Project>> Put([FromBody] CreateProjectRequest? project)
    {
        try
        {
            if (project == null || string.IsNullOrWhiteSpace(project.ProjectName) || string.IsNullOrWhiteSpace(project.BusinessUnit) || string.IsNullOrWhiteSpace(project.Department) || string.IsNullOrWhiteSpace(project.ClientName))
            {
                return BadRequest("ProjectName, BusinessUnit, Department and ClientName must not be empty.");
            }

            var command = new CreateProjectCommand(project.ProjectName, project.BusinessUnit, project.TeamNumber,
                project.Department, project.ClientName);
           Project createdProject = await _mediator.Send(command);
            
           var response = new CreateProjectResponse(createdProject.Id);
           return Created("/Projects/" + createdProject.Id, response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}