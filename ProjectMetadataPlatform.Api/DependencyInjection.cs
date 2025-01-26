using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Api;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the api layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the added dependencies.</returns>
    public static IServiceCollection AddApiDependencies(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<ILogConverter, LogConverter>();
        _ = serviceCollection.AddScoped<IExceptionHandler<PmpException>, BasicExceptionHandler>();
        _ = serviceCollection.AddScoped<IExceptionHandler<ProjectException>, ProjectsExceptionHandler>();
        _ = serviceCollection.AddControllers(options =>
            {
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status401Unauthorized));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status500InternalServerError));
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ErrorResponseMiddleware>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options => options.SuppressMapClientErrors = true);

        return serviceCollection;
    }
}
