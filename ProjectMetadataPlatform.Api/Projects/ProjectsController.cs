using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
/// Endpoints for managing projects.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectsController" /> class.
    /// </summary>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Gets all projects or all projects that match the given search string.
    /// </summary>
    /// <param name="request">The collection of filters to search by.</param>
    /// <param name="search">Search string to filter the projects by.</param>
    /// <returns>All projects or all projects that match the given search string or filters.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetProjectsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        var response = projects.Select(project => new GetProjectsResponse(
            project.Id,
            project.Slug,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.IsArchived));

        return Ok(response);
    }

    /// <summary>
    /// Gets the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns> The project.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(GetProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectResponse>> Get(string slug)
    {
        int? projectId;
        try
        {
            projectId = await GetProjectId(slug);
        }
        catch(Exception e)
        {
             Console.Write(e.GetType());
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return projectId == null ? NotFound($"Project with Slug {slug} not found.") : await Get((int) projectId);
    }

    /// <summary>
    /// Gets the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The project.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            return NotFound(id);
        }

        var response = new GetProjectResponse(
            project.Id,
            project.Slug,
            project.ProjectName,
            project.ClientName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department,
            project.IsArchived,
            project.OfferId,
            project.Company,
            project.CompanyState,
            project.IsmsLevel);

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
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetPlugins(int id)
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

        var response = projectPlugins.Select(plugin
            => new GetPluginResponse(plugin.Plugin!.PluginName, plugin.Url,
                plugin.DisplayName ?? plugin.Plugin.PluginName, plugin.Plugin.Id));

        return Ok(response);
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="404">No project with the given Slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/plugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetPluginsBySlug(string slug)
    {
        int? projectId;
        try
        {
            projectId = await GetProjectId(slug);
        }
        catch(Exception e)
        {
            Console.Write(e.GetType());
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return projectId == null ? NotFound($"Project with Slug {slug} not found.") : await GetPlugins((int) projectId);
    }

    /// <summary>
    ///     Gets all the unarchived plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The unarchived plugins of the project.</returns>
    /// <response code="200">Returns the list of unarchived plugins for the project</response>
    /// <response code="404">If the project with the specified ID is not found</response>
    /// <response code="500">If there was an internal server error while processing the request</response>
    [HttpGet("{id:int}/unarchivedPlugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetUnarchivedPlugins(int id)
    {
        var query = new GetAllUnarchivedPluginsForProjectIdQuery(id);
        IEnumerable<ProjectPlugins> unarchivedProjectPlugins;

        try
        {
            unarchivedProjectPlugins = await _mediator.Send(query);
        }
        catch(ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            return NotFound($"Project with Id {id} not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = unarchivedProjectPlugins
            .Where(plugin => plugin.Plugin != null)
            .Select(plugin => new GetPluginResponse(
                plugin.Plugin!.PluginName,
                plugin.Url,
                plugin.DisplayName ?? plugin.Plugin.PluginName,
                plugin.Plugin.Id
            ));

        return Ok(response);
    }

    /// <summary>
    /// Gets all the unarchived plugins of the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The unarchived plugins of the project.</returns>
    /// <response code="200">All unarchived plugins of the project are returned successfully.</response>
    /// <response code="404">No project with the given slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/unarchivedPlugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetUnarchivedPluginsBySlug(string slug)
    {
        int? projectId;
        try
        {
            projectId = await GetProjectId(slug);
        }
        catch(Exception e)
        {
            Console.Write(e.GetType());
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return projectId == null ? NotFound($"Project with Slug {slug} not found.") : await GetUnarchivedPlugins((int) projectId);
    }

    /// <summary>
    /// Creates a new project or updates the one with given slug.
    /// </summary>
    /// <param name="project">The data of the new project.</param>
    /// <param name="slug">The slug, if an existing project should be overwritten.</param>
    /// <returns>A response containing the id of the created project.</returns>
    /// <response code="201">The Project has been created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="404">The project with the specified slug was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{slug}")]
    [ProducesResponseType(typeof(CreateProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateProjectResponse>> Put([FromBody] CreateProjectRequest project,
        string slug)
    {
        int? projectId;
        try
        {
            projectId = await GetProjectId(slug);
        }
        catch(Exception e)
        {
            Console.Write(e.GetType());
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return projectId == null ? NotFound($"Project with Slug {slug} not found.") : await Put(project, projectId);
    }

    /// <summary>
    /// Creates a new project or updates the one with given id.
    /// </summary>
    /// <param name="project">The data of the new project.</param>
    /// <param name="projectId">The id, if an existing project should be overwritten.</param>
    /// <returns>A response containing the id of the created project.</returns>
    /// <response code="201">The Project has been created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateProjectResponse>> Put([FromBody] CreateProjectRequest project, int? projectId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(project.ProjectName) || string.IsNullOrWhiteSpace(project.BusinessUnit)
                                                               || string.IsNullOrWhiteSpace(project.Department)
                                                               || string.IsNullOrWhiteSpace(project.ClientName))
            {
                return BadRequest("ProjectName, BusinessUnit, Department and ClientName must not be empty.");
            }

            IRequest<int> command = projectId == null
                ? new CreateProjectCommand(project.ProjectName, project.BusinessUnit, project.TeamNumber,
                    project.Department, project.ClientName, project.OfferId, project.Company, project.CompanyState, project.IsmsLevel, (project.PluginList ?? []).Select(p => new ProjectPlugins
                    {
                        PluginId = p.Id,
                        DisplayName = p.DisplayName,
                        Url = p.Url
                    }).ToList())
                : new UpdateProjectCommand(project.ProjectName, project.BusinessUnit, project.TeamNumber,
                    project.Department, project.ClientName, project.OfferId, project.Company, project.CompanyState, project.IsmsLevel, projectId.Value, (project.PluginList ?? []).Select(p => new ProjectPlugins
                    {
                        ProjectId = projectId.Value,
                        PluginId = p.Id,
                        DisplayName = p.DisplayName,
                        Url = p.Url
                    }).ToList(), project.IsArchived);

            var id = await _mediator.Send(command);

            var response = new CreateProjectResponse(id);
            return Created("/Projects/" + id, response);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Retrieves a distinct list of all business units from the projects.
    /// </summary>
    /// <remarks>
    /// This endpoint queries all projects without any filter (empty search string) and extracts the business units.
    /// It then returns a distinct list of these business units. This can be useful for filtering projects by business unit
    /// or simply to obtain an overview of all business units involved in the projects.
    /// </remarks>
    /// <returns>An <see cref="ActionResult"/> containing a list of distinct business units.</returns>
    /// <response code="200">Returns the list of distinct business units successfully.</response>
    /// <response code="500">Indicates an internal error occurred while processing the request.</response>
    [HttpGet("filterData/businessunits")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> GetAllBusinessUnits()
    {
        var query = new GetAllBusinessUnitsQuery();
        IEnumerable<string> businessunits;

        try
        {
            businessunits = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = businessunits;

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a distinct list of all team numbers from the projects.
    /// </summary>
    /// <remarks>
    /// This endpoint queries all projects without any filter (empty search string) and extracts the team numbers.
    /// It then returns a distinct list of these team numbers. This can be useful for filtering projects by team number
    /// or simply to obtain an overview of all team numbers involved in the projects.
    /// </remarks>
    /// <returns>An <see cref="ActionResult"/> containing a list of distinct team numbers.</returns>
    /// <response code="200">Returns the list of distinct team numbers successfully.</response>
    /// <response code="500">Indicates an internal error occurred while processing the request.</response>
    [HttpGet("filterData/teamnumbers")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<int>>> GetAllTeamNumbers()
    {
        var query = new GetAllTeamNumbersQuery();
        IEnumerable<int> teamNumbers;

        try
        {
            teamNumbers = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = teamNumbers;

        return Ok(response);
    }

    /// <summary>
    /// Deletes the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project to delete.</param>
    /// <returns>An ActionResult indicating the result of the delete operation.</returns>
    /// <response code="204">The project was deleted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The project with the specified slug was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{slug}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(string slug)
    {
        int? projectId;
        try
        {
            projectId = await GetProjectId(slug);
        }
        catch(Exception e)
        {
            Console.Write(e.GetType());
            Console.Write(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return projectId == null ? NotFound($"Project with Slug {slug} not found.") : await Delete((int) projectId);

    }

    /// <summary>
    /// Deletes the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project to delete.</param>
    /// <returns>An ActionResult indicating the result of the delete operation.</returns>
    /// <response code="204">The project was deleted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The project with the specified id was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteProjectCommand(id);
        try
        {
            _ = await _mediator.Send(command);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets a project id by its slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The id of the project.</returns>
    private async Task<int?> GetProjectId(string slug)
    {
        var query = new GetProjectIdBySlugQuery(slug);
        return await _mediator.Send(query);
    }
}
