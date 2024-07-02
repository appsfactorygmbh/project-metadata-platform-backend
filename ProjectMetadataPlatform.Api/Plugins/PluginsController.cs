using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Plugins;

/// <summary>
/// Endpoints for managing plugins.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="PluginsController"/>.
    /// </summary>
    /// <param name="mediator"></param>
    public PluginsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> Get([FromQuery] int id)
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
    /// Creates a new plugin with the given name.
    /// </summary>
    /// <param name="request">The request body.</param>
    /// <returns>A HTTP Created Response and the Id of the new Plugin.</returns>
    /// <response code="201">The Plugin was created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    public async Task<ActionResult<CreatePluginResponse>> Put([FromBody] CreatePluginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PluginName))
        {
            return StatusCode(StatusCodes.Status400BadRequest, "PluginName can't be empty or whitespaces");
        }

        var command = new CreatePluginCommand(request.PluginName);

        int pluginId;
        try
        {
            pluginId = await _mediator.Send(command);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = new CreatePluginResponse(pluginId);
        var uri = "/Plugins/" + pluginId;
        return Created(uri, response);
    }
    
    /// <summary>
    /// Updates a global plugin.
    /// </summary>
    /// <param name="pluginId">The id of the plugin to update.</param>
    /// <param name="request">The request body containing the details of the global plugin to be updated.</param>
    /// <returns>The updated version of the Plugin.</returns>
    /// <response code="200">The Plugin was updated successfully.</response>
    /// <response code="404">No Plugin with the requested id was found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{pluginId:int}")]
    public async Task<ActionResult<GetGlobalPluginResponse>> Patch(int pluginId, [FromBody] PatchGlobalPluginRequest request)
    {
        var command = new PatchGlobalPluginCommand(pluginId, request.PluginName, request.IsArchived);

        Plugin? plugin;
        try
        {
            plugin = await _mediator.Send(command);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (plugin == null)
        {
            return NotFound("No Plugin with id " + pluginId + " was found.");
        }

        var response = new GetGlobalPluginResponse(plugin.Id, plugin.PluginName, plugin.IsArchived, []);
        return Ok(response);
    }

    [HttpDelete( "{pluginId:int}" )]
    public async Task<ActionResult <DeleteGlobalPluginResponse>> Delete(int pluginId)
    {
        if (pluginId == 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "PluginId can't be 0");
        }

        var command = new DeleteGlobalPluginCommand(pluginId);
        
        try
        {
            var result = await _mediator.Send(command);
            if (result == 0)
            {
                return NotFound("No Plugin with id " + pluginId + " was found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var response = new DeleteGlobalPluginResponse(pluginId, true);

        return Ok(response);
    }
}
