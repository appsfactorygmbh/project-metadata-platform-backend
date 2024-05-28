using System;

namespace ProjectMetadataPlatform.Api.Models.WeatherForecast;

public record GetWeatherForecastResponse(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}