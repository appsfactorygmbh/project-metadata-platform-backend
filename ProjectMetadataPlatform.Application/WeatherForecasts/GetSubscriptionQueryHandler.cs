using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.WeatherForecasts;

namespace ProjectMetadataPlatform.Application.WeatherForecasts;

/// <inheritdoc />
public class GetSubscriptionQueryHandler : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetSubscriptionQueryHandler"/>.
    /// </summary>
    public GetSubscriptionQueryHandler(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    /// <inheritdoc />
    public Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        return _weatherForecastRepository.GetWeatherForecastsAsync(request.Count);
    }
}