using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class ProjectManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "businessUnit": "BU1",
          "teamNumber": 3,
          "department": "testDepartment",
          "clientName": "testClient",
          "offerId": "testId",
          "company": "testCompany",
          "companyState": "EXTERNAL",
          "ismsLevel": "NORMAL"

        }
        """);

    private static readonly StringContent CreateRequest2 = StringContent(
        """
        {
          "projectName": "otherTestProject2",
          "businessUnit": "BU2",
          "teamNumber": 4,
          "department": "testDepartment2",
          "clientName": "testClient2",
          "offerId": "testId2",
          "company": "testCompany2",
          "companyState": "EXTERNAL",
          "ismsLevel": "VERY_HIGH"
        }
        """);

    private static readonly StringContent UpdateRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "businessUnit": "BU2",
          "teamNumber": 7,
          "department": "testDepartment2",
          "clientName": "testClient2",
          "offerId": "testId2",
          "company": "testCompany2",
          "companyState": "INTERNAL",
          "ismsLevel": "HIGH"
        }
        """);

    private static readonly StringContent UpdateisArchivedRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "businessUnit": "BU1",
          "teamNumber": 3,
          "department": "testDepartment",
          "clientName": "testClient",
          "isArchived": true,
          "offerId": "testId",
          "company": "testCompany",
          "companyState": "EXTERNAL",
          "ismsLevel": "NORMAL"
        }
        """);

    [Test]
    public async Task CreateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var putResponse = await client.PutAsync("/Projects", CreateRequest);

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
        rootElement.GetProperty("offerId").GetString().Should().Be("testId");
        rootElement.GetProperty("company").GetString().Should().Be("testCompany");
        rootElement.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        rootElement.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
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
        var putResponse = await client.PutAsync("/Projects", CreateRequest);
        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        putResponse = await client.PutAsync("/Projects", CreateRequest2);
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
        firstProject.GetProperty("offerId").GetString().Should().Be("testId");
        firstProject.GetProperty("company").GetString().Should().Be("testCompany");
        firstProject.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        firstProject.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        firstProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var secondProject = rootElement[1];
        secondProject.GetProperty("projectName").GetString().Should().Be("otherTestProject2");
        secondProject.GetProperty("businessUnit").GetString().Should().Be("BU2");
        secondProject.GetProperty("teamNumber").GetInt32().Should().Be(4);
        secondProject.TryGetProperty("department", out _).Should().BeFalse();
        secondProject.GetProperty("clientName").GetString().Should().Be("testClient2");
        secondProject.GetProperty("offerId").GetString().Should().Be("testId2");
        secondProject.GetProperty("company").GetString().Should().Be("testCompany2");
        secondProject.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        secondProject.GetProperty("ismsLevel").GetString().Should().Be("VERY_HIGH");
        secondProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }

    [Test]
    public async Task UpdateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var projectId = (await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created))
            .GetProperty("id").GetInt32()!;

        var updateResponse = await client.PutAsync($"/Projects?projectId="+projectId, UpdateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updateResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(updateResponse.Headers.Location);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        rootElement.GetProperty("businessUnit").GetString().Should().Be("BU2");
        rootElement.GetProperty("teamNumber").GetInt32().Should().Be(7);
        rootElement.GetProperty("department").GetString().Should().Be("testDepartment2");
        rootElement.GetProperty("clientName").GetString().Should().Be("testClient2");
        rootElement.GetProperty("offerId").GetString().Should().Be("testId2");
        rootElement.GetProperty("company").GetString().Should().Be("testCompany2");
        rootElement.GetProperty("companyState").GetString().Should().Be("INTERNAL");
        rootElement.GetProperty("ismsLevel").GetString().Should().Be("HIGH");
        rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(2);

        logs[0].GetProperty("logMessage").GetString().Should().Be(
            "admin created a new project with properties: ProjectName = testProject, Slug = testproject, BusinessUnit = BU1, Department = testDepartment, ClientName = testClient, TeamNumber = 3, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL");

        logs[1].GetProperty("logMessage").GetString().Should().Be(
            "admin updated project testProject:  set BusinessUnit from BU1 to BU2,  set TeamNumber from 3 to 7,  set Department from testDepartment to testDepartment2,  set ClientName from testClient to testClient2,  set OfferId from testId to testId2,  set Company from testCompany to testCompany2,  set CompanyState from EXTERNAL to INTERNAL,  set IsmsLevel from NORMAL to HIGH");
    }

    [Test]
    public async Task DeleteProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var projectId = (await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created))
            .GetProperty("id").GetInt32()!;
        var projects = await ToJsonElement(client.GetAsync($"/Projects/"));
        var count = projects.GetArrayLength();
        await client.PutAsync($"/Projects?projectId="+projectId, UpdateisArchivedRequest);

        (await client.DeleteAsync($"/Projects/{projectId}")).StatusCode.Should().Be(HttpStatusCode.NoContent);

         var projects2 = await ToJsonElement(client.GetAsync($"/Projects/"));
         projects2.GetArrayLength().Should().Be(count-1);

         var logs = await ToJsonElement(client.GetAsync("/Logs"));

         logs.GetArrayLength().Should().Be(3);

         logs[0].GetProperty("logMessage").GetString().Should().Be(
             "admin created a new project with properties: ProjectName = testProject, Slug = testproject, BusinessUnit = BU1, Department = testDepartment, ClientName = testClient, TeamNumber = 3, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL");
         logs[1].GetProperty("logMessage").GetString().Should().Be(
             "admin archived project testProject");
         logs[2].GetProperty("logMessage").GetString().Should().Be(
             "admin removed project testProject");


    }

}
