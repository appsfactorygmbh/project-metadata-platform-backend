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
    /// Retrieves all projects.
    /// </summary>
    /// <returns>All projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> Get()
    {
        var query = new GetAllProjectsQuery();
        IEnumerable<Project> projects;
        try
        {
            projects = await _mediator.Send(query);
        }
        catch 
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = projects.Select(project => new GetProjectsResponse(
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
    public async Task<ActionResult<GetProjectsResponse>> Get(int id)
    {
        var query = new GetProjectQuery(id);
        Project project;
        try
        {
            project = await _mediator.Send(query);
            
            
        }
        catch 
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        if(project == null)
        {
            return NotFound(project);
        }
        var response =  new GetProjectResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department);

        return  Ok(response);
    }
}