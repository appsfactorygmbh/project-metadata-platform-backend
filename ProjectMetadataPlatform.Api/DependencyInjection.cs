using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs;

namespace ProjectMetadataPlatform.Api;

/// <summary>
///     Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Adds the necessary dependencies for the api layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddApiDependencies(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<ILogConverter, LogConverter>();
        return serviceCollection;
    }
}
