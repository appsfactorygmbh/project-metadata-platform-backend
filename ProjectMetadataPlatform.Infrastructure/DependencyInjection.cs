using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.WeatherForecasts;

namespace ProjectMetadataPlatform.Infrastructure;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the infrastructure layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
            options.UseSqlite("Data Source = Database.db"));

        serviceCollection.AddScoped<IProjectsRepository, ProjectsRepository>();
        
        return serviceCollection;
    }
}