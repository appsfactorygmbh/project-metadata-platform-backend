using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Api.Auth.Models;

namespace ProjectMetadataPlatform.IntegrationTests;

public class IntegrationTestsBase : IDisposable
{
    private readonly PmpWebApplicationFactory _factory = new();

    public HttpClient CreateClient() => _factory.CreateClient();

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    protected async Task<string> GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/auth/basic", new { Username = "admin", Password = "admin" });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {content!.AccessToken}");

        return content.AccessToken;
    }
}
