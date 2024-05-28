using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.WeatherForecasts;

namespace ProjectMetadataPlatform.Infrastructure.WeatherForecasts;

/// <inheritdoc />
public class WeatherForecastRepository : IWeatherForecastRepository
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    
    /// <inheritdoc />
    public Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(int count)
    {
        var random = new Random();

        var result =  Enumerable.Range(1, count)
            .Select(index => new WeatherForecast(
                DateTime.Now.AddDays(index),
                random.Next(-20, 55),
                Summaries[random.Next(Summaries.Length)]));

        return Task.FromResult(result);
    }
}