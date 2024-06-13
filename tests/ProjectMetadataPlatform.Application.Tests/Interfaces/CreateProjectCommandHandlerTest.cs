using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

[TestFixture]
public class CreateProjectCommandHandlerTest
{
    private CreateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new CreateProjectCommandHandler(_mockProjectRepo.Object);
    }
    
    [Test]
    public async Task CreateProject_Test()
    {
        // prepare
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client"
        };
        _mockProjectRepo.Setup(m => m.CreateProject(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(exampleProject);
        
        var result = await _handler.Handle(new CreateProjectCommand("Example Project", "Example Business Unit", 1, "Example Department", "Example Client"), It.IsAny<CancellationToken>());
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProjectName, Is.EqualTo("Example Project"));
        Assert.That(result.BusinessUnit, Is.EqualTo("Example Business Unit"));
        Assert.That(result.TeamNumber, Is.EqualTo(1));
        Assert.That(result.ClientName, Is.EqualTo("Example Client"));
        Assert.That(result.Department, Is.EqualTo("Example Department"));
    }

}
