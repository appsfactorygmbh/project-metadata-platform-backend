using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class CreateProjectCommandHandlerTest
{
    private CreateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ISlugHelper> _mockSlugHelper;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSlugHelper = new Mock<ISlugHelper>();
        _handler = new CreateProjectCommandHandler(_mockProjectRepo.Object, _mockPluginRepo.Object, _mockLogRepo.Object, _mockUnitOfWork.Object, _mockSlugHelper.Object);
    }

    [Test]
    public async Task CreateProject_Test()
    {
        // prepare
        var plugins = new List<ProjectPlugins>();
        plugins.Add(new ProjectPlugins
        {
            Url = "http://example.com",
            PluginId = 200
        });
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            ProjectPlugins = plugins
        };
        _mockProjectRepo.Setup(m => m.Add(It.IsAny<Project>())).Callback<Project>(p => p.Id = 1);
        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);
        _mockSlugHelper.Setup(m => m.GenerateSlug(It.IsAny<string>())).Returns("example_project");
        // act

        int result =
            await _handler.Handle(
                new CreateProjectCommand("Example Project", "Example Business Unit", 1, "Example Department",
                    "Example Client", plugins), It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(m => m.AddProjectLogForCurrentUser(It.IsAny<Project>(), Action.ADDED_PROJECT,It.IsAny<List<LogChange>>()), Times.Once);
        _mockLogRepo.Verify(m => m.AddProjectLogForCurrentUser(It.IsAny<Project>(), Action.ADDED_PROJECT_PLUGIN, It.IsAny<List<LogChange>>()), Times.Once);
    }
}
