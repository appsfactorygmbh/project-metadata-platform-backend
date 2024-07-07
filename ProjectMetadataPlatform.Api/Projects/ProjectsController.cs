using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
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
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> Get([FromQuery] ProjectFilterRequest? request, string? search = " ")
    {
        var query = new GetAllProjectsQuery(request, search);
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
    ///     Gets projects with an optional search string for the project name.
    ///     If the search string is set, only projects whose names contain the search string are returned.
    /// </summary>
    /// <param name="searchString">The optional search string for the project name.</param>
    /// <returns>The list of projects.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="404">No projects found matching the search criteria.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GetProjectResponse>>> SearchProjects([FromQuery] string? searchString)
    {
        var query = new SearchProjectsQuery(searchString);
        IEnumerable<Project> projects;
        try
        {
            projects = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (projects == null || !projects.Any())
        {
            return NotFound();
        }

        var response = projects.Select(project => new GetProjectResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department));

        return Ok(response);
    }


    /// <summary>
    ///     Gets projects with an optional search string for the client name.
    ///     If the search string is set, only projects whose client names contain the search string are returned.
    /// </summary>
    /// <param name="clientNameSearchString">The optional search string for the client name.</param>
    /// <returns>The list of projects.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="404">No projects found matching the search criteria.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("searchByClientName")]
    public async Task<ActionResult<IEnumerable<GetProjectResponse>>> SearchProjectsByClientName([FromQuery] string? clientNameSearchString)
    {
        var query = new SearchProjectsClientNameQuery(clientNameSearchString);
        IEnumerable<Project> projects;
        try
        {
            projects = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (projects == null || !projects.Any())
        {
            return NotFound();
        }

        var response = projects.Select(project => new GetProjectResponse(
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department));

        return Ok(response);
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}/plugins")]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetPlugins( int id)
    {
        var query = new GetAllPluginsForProjectIdQuery(id);
        IEnumerable<ProjectPlugins> projectPlugins;
        try
        {
            projectPlugins = await _mediator.Send(query);
        }
        catch
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        IEnumerable<GetPluginResponse> response = projectPlugins.Select(plugin
            => new GetPluginResponse(plugin.Plugin.PluginName, plugin.Url,
                plugin.DisplayName ?? plugin.Plugin.PluginName));

        return Ok(response);
    }

        /// <summary>
    /// Gets all projects with the specified business units.
    /// </summary>
    /// <param name="businessUnits">A list of business units to filter the projects by.</param>
    /// <returns>A list of projects that belong to the specified business units.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="400">The business units parameter is empty.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("BusinessUnits")]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> GetByBusinessUnits([FromQuery] List<string>? businessUnits)
    {
        if (businessUnits == null || businessUnits.Count == 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Business units cannot be empty");
        }

        var query = new GetProjectsByBusinessUnitsQuery(businessUnits);
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
    /// Gets all projects with the specified team numbers.
    /// </summary>
    /// <param name="teamNumbers">A list of team numbers to filter the projects by.</param>
    /// <returns>A list of projects that belong to the specified team numbers.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="400">The team numbers parameter is empty.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("TeamNumber")]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> GetByTeamNumbers([FromQuery] List<int>? teamNumbers)
    {
        if (teamNumbers == null || teamNumbers.Count == 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Team numbers cannot be empty");
        }

        var query = new GetProjectsByTeamNumbersQuery(teamNumbers);
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
    ///     Creates a new project.
    /// </summary>
    /// <param name="project">The data of the new project.</param>
    /// <returns>A response containing the id of the created project.</returns>
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
