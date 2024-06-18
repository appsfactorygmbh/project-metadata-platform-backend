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
/// Controller to get Plugins for given Project.
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
    /// Get all the plugins for a given project id.
    /// </summary>
    /// <param name="id">selects the project</param>
    /// <returns>An HTML ok response with List of Plugins.</returns>
    /// <response code="200">Plugins are returned successfully</response>
    /// <response code="500">Internal Server Error</response>
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
    /// Creates a new plugin with the given name
    /// </summary>
    /// <param name="request">The request body.</param>
    /// <returns>A HTTP Created Response and the Id of the new Plugin</returns>
    /// <response code="201">Plugin is created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPut]
    public async Task<ActionResult<Plugin>> Put([FromBody] CreatePluginRequest request)
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
}
