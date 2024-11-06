using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests.Utilities;

public class IntegrationTestsBase : IDisposable
{
    private readonly PmpWebApplicationFactory _factory = new();

    protected HttpClient CreateClient() => _factory.CreateClient();

    [SetUp]
    public async Task BaseSetup()
    {
        var platformDbContext = _factory.Services.GetRequiredService<ProjectMetadataPlatformDbContext>();
        var allEntitiesPlugins = platformDbContext.Plugins.ToList();
        var allEntitiesProjects = platformDbContext.Projects.ToList();
        var allEntitiesProjectsPlugins = platformDbContext.ProjectPluginsRelation.ToList();
        platformDbContext.Plugins.RemoveRange(allEntitiesPlugins);
        platformDbContext.Projects.RemoveRange(allEntitiesProjects);
        platformDbContext.ProjectPluginsRelation.RemoveRange(allEntitiesProjectsPlugins);

        await platformDbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    protected static async Task GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/auth/basic", new { Username = "admin", Password = "admin" });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {content!.AccessToken}");

    }
}
