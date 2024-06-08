using Microsoft.Extensions.DependencyInjection;

namespace ProjectMetadataPlatform.Application;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the application layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));
    }
}