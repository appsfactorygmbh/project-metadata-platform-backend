using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
[Route("[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="PluginsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public PluginsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new plugin with the given name.
    /// </summary>
    /// <param name="request">The request body.</param>
    /// <returns>An HTTP Created Response and the Id of the new Plugin.</returns>
    /// <response code="201">The Plugin was created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreatePluginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatePluginResponse>> Put([FromBody] CreatePluginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PluginName))
        {
            return StatusCode(StatusCodes.Status400BadRequest, "PluginName can't be empty or whitespaces");
        }

        var command = new CreatePluginCommand(request.PluginName, request.IsArchived, request.Keys);

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
    [ProducesResponseType(typeof(GetGlobalPluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetGlobalPluginResponse>> Patch(
        int pluginId,
        [FromBody] PatchGlobalPluginRequest request)
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

        var response = new GetGlobalPluginResponse(plugin.PluginName, plugin.Id, plugin.IsArchived, [], plugin.BaseUrl);
        return Ok(response);
    }

    /// <summary>
    /// Gets all global plugins.
    /// </summary>
    /// <returns>All global plugins.</returns>
    /// <response code="200">All global plugins are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetGlobalPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GetGlobalPluginResponse>>> GetGlobal()
    {
        var query = new GetGlobalPluginsQuery();
        IEnumerable<Plugin> plugins;
        try
        {
            plugins = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        string[] keys = [];
        var response = plugins.Select(plugin => new GetGlobalPluginResponse(
            plugin.PluginName,
            plugin.Id,
            plugin.IsArchived,
            keys,
            plugin.BaseUrl
            ));

        return Ok(response);
    }

    /// <summary>
    /// Deletes a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The unique identifier of the plugin to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, with an <see cref="ActionResult"/> that
    /// contains a <see cref="DeleteGlobalPluginResponse"/> on success, or an error message on failure.
    /// </returns>
    /// <response code="200">OK if the plugin was successfully deleted.</response>
    /// <response code="400">Bad Request if the pluginId is smaller than or equal to 0, or if the plugin with the given id is not archived.</response>
    /// <response code="404">Not Found if no plugin with the specified ID was found.</response>
    /// <response code="500">Internal Server Error if an unexpected exception occurs.</response>
    [HttpDelete("{pluginId:int}")]
    [ProducesResponseType(typeof(DeleteGlobalPluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeleteGlobalPluginResponse>> Delete(int pluginId)
    {
        if (pluginId <= 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "PluginId can't be 0");
        }
        var command = new DeleteGlobalPluginCommand(pluginId);

        try
        {
            var success = await _mediator.Send(command);
            if (success == null)
            {
                return NotFound("No Plugin with id " + pluginId + " was found.");
            }
            else if ((bool)!success)
            {
                return StatusCode(StatusCodes.Status400BadRequest,"the plugin was not archived");
            }
            return Ok(new DeleteGlobalPluginResponse(pluginId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
