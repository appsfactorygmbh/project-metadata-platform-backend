using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class CreateProjectCommandHandlerTest
{
    private CreateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _handler = new CreateProjectCommandHandler(_mockProjectRepo.Object, _mockPluginRepo.Object, _mockLogRepo.Object);
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
        _mockProjectRepo.Setup(m => m.Add(It.IsAny<Project>())).Callback<Project>(p => p.Id = 1)
            .Returns(Task.CompletedTask);

        // act

        int result =
            await _handler.Handle(
                new CreateProjectCommand("Example Project", "Example Business Unit", 1, "Example Department",
                    "Example Client", []), It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(1));

    }
}
