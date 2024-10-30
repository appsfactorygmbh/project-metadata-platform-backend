using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests;

public class PmpWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("PMP_DB_URL", " ");
        Environment.SetEnvironmentVariable("PMP_DB_PORT", " ");
        Environment.SetEnvironmentVariable("PMP_DB_USER", " ");
        Environment.SetEnvironmentVariable("PMP_DB_PASSWORD", " ");
        Environment.SetEnvironmentVariable("PMP_DB_NAME", " ");
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", " ");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", " ");
        Environment.SetEnvironmentVariable("JWT_ISSUER_SIGNING_KEY", " ");
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", " ");
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", " ");
        Environment.SetEnvironmentVariable("PMP_MIGRATE_DB_ON_STARTUP", "true");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<ProjectMetadataPlatformDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbConnection));

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
                options.UseSqlite("Datasource=unittest-db.db"));
        });

        builder.UseEnvironment("Production");
    }
}
