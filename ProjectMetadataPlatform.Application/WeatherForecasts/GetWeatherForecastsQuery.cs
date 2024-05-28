using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.WeatherForecasts;

namespace ProjectMetadataPlatform.Application.WeatherForecasts;

/// <summary>
/// Query to get weather forecasts.
/// </summary>
/// <param name="Count">How many weather forecasts to retrieve.</param>
public record GetWeatherForecastsQuery(int Count) : IRequest<IEnumerable<WeatherForecast>>;