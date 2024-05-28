using ProjectMetadataPlatform.Domain.WeatherForecasts;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for weather forecasts.
/// </summary>
public interface IWeatherForecastRepository
{
    /// <summary>
    /// Returns a collection of weather forecasts.
    /// </summary>
    /// <param name="count">How many weather forecasts should be returned.</param>
    /// <returns>The weather forecasts.</returns>
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(int count);
}