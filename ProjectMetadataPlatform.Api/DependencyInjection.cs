using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Domain.Errors;

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
        _ = serviceCollection.AddScoped<IExceptionHandler<PmpException>, BasicExceptionHandler>();
        _ = serviceCollection.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
        });
        return serviceCollection;
    }
}
