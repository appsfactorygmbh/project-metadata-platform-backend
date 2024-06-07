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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectPlugins>>> Get([FromQuery] int id)
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
}