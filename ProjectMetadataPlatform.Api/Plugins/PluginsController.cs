using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Plugins;

/// <summary>
/// Controller for weather forecasts.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="mediator"></param>
    public PluginsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the given number of weather forecasts.
    /// </summary>
    /// <param name="id">select for what Project</param>
    /// <returns>The weather forecasts.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plugin>>> Get([FromQuery] int id)
    {
       var query = new GetAllPluginsForProjectIdQuery(id);
       var plugins = await _mediator.Send(query);
       var response = plugins.Select(plugin => new GetPluginResponse(plugin.PluginName, plugin.Url));
       return Ok(response);
    }
}