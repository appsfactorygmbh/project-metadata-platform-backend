using System;

namespace ProjectMetadataPlatform.Domain.WeatherForecasts;

/// <summary>
/// The weather forecast for a specific date.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="TemperatureC">The predicted temperature in Celsius.</param>
/// <param name="Summary">The summary of the weather.</param>
public record WeatherForecast(DateTime Date, int TemperatureC, string Summary);