using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class ProjectsWorkflow : IntegrationTestsBase
{
    private const string CREATE_PROJECT_REQUEST = """
                                                  {
                                                    "projectName": "testProject",
                                                    "businessUnit": "BU1",
                                                    "teamNumber": 3,
                                                    "department": "testDepartment",
                                                    "clientName": "testClient"
                                                  }
                                                  """;

    private const string CREATE_PROJECT_REQUEST2 = """
                                                  {
                                                    "projectName": "testProject2",
                                                    "businessUnit": "BU2",
                                                    "teamNumber": 4,
                                                    "department": "testDepartment2",
                                                    "clientName": "testClient2"
                                                  }
                                                  """;

    [Test]
    public async Task CreateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var putResponse = await client.PutAsync("/Projects", new StringContent(CREATE_PROJECT_REQUEST, System.Text.Encoding.UTF8, "application/json"));

        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        putResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(putResponse.Headers.Location);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        rootElement.GetProperty("businessUnit").GetString().Should().Be("BU1");
        rootElement.GetProperty("teamNumber").GetInt32().Should().Be(3);
        rootElement.GetProperty("department").GetString().Should().Be("testDepartment");
        rootElement.GetProperty("clientName").GetString().Should().Be("testClient");
        rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }

    [Test]
    public async Task CreateMultipleProjects()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var putResponse = await client.PutAsync("/Projects", new StringContent(CREATE_PROJECT_REQUEST, System.Text.Encoding.UTF8, "application/json"));
        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        putResponse = await client.PutAsync("/Projects", new StringContent(CREATE_PROJECT_REQUEST2, System.Text.Encoding.UTF8, "application/json"));
        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await client.GetAsync("/Projects");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetArrayLength().Should().Be(2);

        var firstProject = rootElement[0];
        firstProject.GetProperty("projectName").GetString().Should().Be("testProject");
        firstProject.GetProperty("businessUnit").GetString().Should().Be("BU1");
        firstProject.GetProperty("teamNumber").GetInt32().Should().Be(3);
        firstProject.TryGetProperty("department", out _).Should().BeFalse();
        firstProject.GetProperty("clientName").GetString().Should().Be("testClient");
        firstProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var secondProject = rootElement[1];
        secondProject.GetProperty("projectName").GetString().Should().Be("testProject2");
        secondProject.GetProperty("businessUnit").GetString().Should().Be("BU2");
        secondProject.GetProperty("teamNumber").GetInt32().Should().Be(4);
        firstProject.TryGetProperty("department", out _).Should().BeFalse();
        secondProject.GetProperty("clientName").GetString().Should().Be("testClient2");
        secondProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }
}
