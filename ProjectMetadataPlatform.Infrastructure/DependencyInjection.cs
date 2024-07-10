using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Plugins;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ProjectMetadataPlatform.Infrastructure;

/// <summary>
///     Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Adds the necessary dependencies for the infrastructure layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContextWithPostgresConnection();
        serviceCollection.ConfigureAuth();
        _ = serviceCollection.AddScoped<IPluginRepository, PluginRepository>();
        _ = serviceCollection.AddScoped<IProjectsRepository, ProjectsRepository>();
        _ = serviceCollection.AddScoped<IAuthRepository, AuthRepository>();

        return serviceCollection;
    }

    private static void AddDbContextWithPostgresConnection(this IServiceCollection serviceCollection)
    {
        var url = Environment.GetEnvironmentVariable("PMP_DB_URL");
        var port = Environment.GetEnvironmentVariable("PMP_DB_PORT");
        var user = GetEnvVarOrLoadFromFile("PMP_DB_USER");
        var password = GetEnvVarOrLoadFromFile("PMP_DB_PASSWORD");
        var database = GetEnvVarOrLoadFromFile("PMP_DB_NAME");

        var connectionString = $"Host={url};Port={port};User Id={user};Password={password};Database={database}";

        serviceCollection.AddDbContext<ProjectMetadataPlatformDbContext>(options
            => options.UseNpgsql(connectionString));

        static string GetEnvVarOrLoadFromFile(string envVarName)
        {
            var value = Environment.GetEnvironmentVariable(envVarName);

            if (value is not null)
            {
                return value;
            }

            var path = Environment.GetEnvironmentVariable(envVarName + "_FILE")
                       ?? throw new InvalidOperationException($"Either {envVarName} or {envVarName}_FILE must be configured");

            return File.ReadAllText(path);
        }
    }

    /// <summary>
    ///    Configures the authentication for the project.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    private static void ConfigureAuth(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ProjectMetadataPlatformDbContext>()
            .AddDefaultTokenProviders();

        _ = serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //should also get this from the environment
                    ValidIssuer = "ValidIssuer",
                    ValidAudience = "ValidAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKeyThatIsAtLeast257BitLong@345"))
                };
            });
    }

    /// <summary>
    /// Migrates the database.
    /// </summary>
    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        if (Environment.GetEnvironmentVariable("PMP_MIGRATE_DB_ON_STARTUP")?.ToLowerInvariant() is "true")
        {
            using var serviceScope = serviceProvider.CreateScope();
            var services = serviceScope.ServiceProvider;

            var myDependency = services.GetRequiredService<ProjectMetadataPlatformDbContext>();
            myDependency.Database.Migrate();
        }
    }
}
