using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Models.WeatherForecast;
using ProjectMetadataPlatform.Application.WeatherForecasts;

namespace ProjectMetadataPlatform.Api.WeatherForecasts;

/// <summary>
/// Controller for weather forecasts.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="mediator"></param>
    public WeatherForecastController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the given number of weather forecasts.
    /// </summary>
    /// <param name="count">How many weather forecasts to retrieve.</param>
    /// <returns>The weather forecasts.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetWeatherForecastResponse>>> Get([FromQuery] int count)
    {
        var query = new GetWeatherForecastsQuery(count);

        var weatherForecasts = await _mediator.Send(query);

        var responses = weatherForecasts.Select(weatherForecast => new GetWeatherForecastResponse(
            weatherForecast.Date,
            weatherForecast.TemperatureC,
            weatherForecast.Summary));

        return Ok(responses);
    }
}