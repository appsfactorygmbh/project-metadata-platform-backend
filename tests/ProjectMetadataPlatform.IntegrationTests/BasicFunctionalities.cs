using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

[TestFixture]
public class BasicFunctionalities : IntegrationTestsBase
{
    [Test]
    public async Task SwaggerShouldBeAccessible()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/swagger");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestCase("/Projects", "GET")]
    [TestCase("/Projects/1", "GET")]
    [TestCase("/Projects/1/plugins", "GET")]
    [TestCase("/Projects/filterData/businessunits", "GET")]
    [TestCase("/Projects/filterData/teamnumbers", "GET")]
    [TestCase("/Projects", "PUT")]
    [TestCase("/Plugins", "GET")]
    [TestCase("/Plugins", "PUT")]
    [TestCase("/Plugins/1", "PATCH")]
    [TestCase("/Plugins/1", "DELETE")]
    public async Task AllRelevantEndpointsShouldReturnUnauthorizedWhenNotAuthenticated(string endpoint, string method)
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), endpoint));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
