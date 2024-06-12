using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [HttpPut]
    public async Task<ActionResult<Plugin>> Put([FromBody] CreatePluginRequest request)
    {
        var command = new CreatePluginCommand(request.PluginName);
        
       Plugin plugin;
       try
       {
           plugin = await _mediator.Send(command);
       }
       catch 
       {
           return new StatusCodeResult(StatusCodes.Status500InternalServerError);
       }
        
        var response = new CreatePluginResponse(plugin.Id);
        var uri = Request.GetDisplayUrl() + "/" + plugin.Id;
        return Created(uri, response);
    }
}
