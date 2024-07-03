using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

[TestFixture]
public class CreateProjectCommandHandlerTest
{

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new CreateProjectCommandHandler(_mockProjectRepo.Object);
    }
    private CreateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

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
        _mockProjectRepo.Setup(m => m.AddOrUpdate(It.IsAny<Project>())).Callback<Project>(p => p.Id = 1)
            .Returns(Task.CompletedTask);

        // act

        int result =
            await _handler.Handle(
                new CreateProjectCommand("Example Project", "Example Business Unit", 1, "Example Department",
                    "Example Client"), It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(1));

    }
}
