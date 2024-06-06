using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Tests;

[TestFixture]
public class ProjectInitializationTests
{
    [Test]
    public void Project_ShouldInitializeCorrectly()
    {
        // Arrange
        int expectedId = 1;
        string expectedProjectName = "Tagesschau";
        string expectedClientName = "ARD";
        string expectedBusinessUnit = "Development";
        int expectedTeamNumber = 42;
        string expectedDepartment = "ABC";

        // Act
        var project = new Project
        {
            Id = expectedId,
            ProjectName = expectedProjectName,
            ClientName = expectedClientName,
            BusinessUnit = expectedBusinessUnit,
            TeamNumber = expectedTeamNumber,
            Department = expectedDepartment
        };

        // Assert
        Assert.AreEqual(expectedId, project.Id);
        Assert.AreEqual(expectedProjectName, project.ProjectName);
        Assert.AreEqual(expectedClientName, project.ClientName);
        Assert.AreEqual(expectedBusinessUnit, project.BusinessUnit);
        Assert.AreEqual(expectedTeamNumber, project.TeamNumber);
        Assert.AreEqual(expectedDepartment, project.Department);
    }
}
