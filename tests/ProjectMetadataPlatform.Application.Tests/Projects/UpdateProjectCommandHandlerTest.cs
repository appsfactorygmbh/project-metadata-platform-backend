using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class UpdateProjectCommandHandlerTest
{
    private UpdateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateProjectCommandHandler(_mockProjectRepo.Object, _mockPluginRepo.Object, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task UpdateProject_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            ProjectPlugins = new List<ProjectPlugins>()
        };
        var examplePlugin = new Plugin
        {
            Id = 100,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy"
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1))
            .ReturnsAsync(exampleProject);

        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new UpdateProjectCommand(exampleProject.ProjectName, exampleProject.BusinessUnit, exampleProject.TeamNumber, exampleProject.Department, exampleProject.ClientName, exampleProject.Id, projectPluginList), It.IsAny<CancellationToken>());
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateProjectNotFound_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            ProjectPlugins = new List<ProjectPlugins>()
        };
        var examplePlugin = new Plugin
        {
            Id = 100,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy"
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.CheckProjectExists(1))
            .ReturnsAsync(false);

        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>()))
            .ReturnsAsync(true);

        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(new UpdateProjectCommand(
                    exampleProject.ProjectName,
                    exampleProject.BusinessUnit,
                    exampleProject.TeamNumber,
                    exampleProject.Department,
                    exampleProject.ClientName,
                    exampleProject.Id,
                    projectPluginList),
                CancellationToken.None)
        );
        Assert.That(exception.Message, Is.EqualTo("Project does not exist."));
    }

    [Test]
    public async Task UpdatePlugintNotFound_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            ProjectPlugins = new List<ProjectPlugins>()
        };
        var examplePlugin = new Plugin
        {
            Id = 200,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy"
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1))
            .ReturnsAsync(exampleProject);

        _mockPluginRepo.Setup(m => m.CheckPluginExists(100))
            .ReturnsAsync(false);

        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(new UpdateProjectCommand(
                    exampleProject.ProjectName,
                    exampleProject.BusinessUnit,
                    exampleProject.TeamNumber,
                    exampleProject.Department,
                    exampleProject.ClientName,
                    exampleProject.Id,
                    projectPluginList),
                CancellationToken.None)
        );
        Assert.That(exception.Message, Is.EqualTo("The Plugins with these ids do not exist: 100"));
    }

    [Test]
    public async Task UpdatesProjectInformation()
    {
        //Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            ClientName = "DB",
            BusinessUnit = "Unit 1",
            TeamNumber = 1,
            Department = "Department 1",
            ProjectPlugins = []
        };

        var updateCommand = new UpdateProjectCommand("DB App",
            "Unit 2",
            2,
            "Department 2",
            "Deutsche Bahn",
            1,
            new List<ProjectPlugins>());

        _mockProjectRepo.Setup(repository => repository.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);

        //Act
        await _handler.Handle(updateCommand, CancellationToken.None);

        //Assert
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());
        Assert.Multiple(() =>
        {
            Assert.That(project.BusinessUnit, Is.EqualTo("Unit 2"));
            Assert.That(project.TeamNumber, Is.EqualTo(2));
            Assert.That(project.Department, Is.EqualTo("Department 2"));
            Assert.That(project.ClientName, Is.EqualTo("Deutsche Bahn"));
            Assert.That(project.ProjectName, Is.EqualTo("DB App"));
        });
    }

    [Test]
    public async Task UpdatesProjectPlugins()
    {
        //Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            ClientName = "DB",
            BusinessUnit = "Unit 1",
            TeamNumber = 1,
            Department = "Department 1",
            ProjectPlugins =
            [
                new ProjectPlugins { PluginId = 1, Url = "http://example.com", DisplayName = "Example Plugin" },
                new ProjectPlugins { PluginId = 1, Url = "http://another-example.com", DisplayName = "Another Plugin" },
                new ProjectPlugins { PluginId = 2, Url = "http://different-example.com", DisplayName = "Different Plugin" }
            ]
        };

        var updateCommand = new UpdateProjectCommand("DB App",
            "Unit 2",
            2,
            "Department 2",
            "Deutsche Bahn",
            1,
            [
                new ProjectPlugins { PluginId = 1, Url = "http://example.com", DisplayName = "Example Plugin" },
                new ProjectPlugins { PluginId = 1, Url = "http://another-example.com", DisplayName = "Another example Plugin" },
                new ProjectPlugins { PluginId = 3, Url = "http://example2.com", DisplayName = "Example 2 Plugin" }
            ]);

        _mockProjectRepo.Setup(repository => repository.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);
        _mockPluginRepo.Setup(repository => repository.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);

        //Act
        await _handler.Handle(updateCommand, CancellationToken.None);

        //Assert
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());

        Assert.That(project.ProjectPlugins, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(0).PluginId, Is.EqualTo(1));
            Assert.That(project.ProjectPlugins.ElementAt(0).Url, Is.EqualTo("http://example.com"));
            Assert.That(project.ProjectPlugins.ElementAt(0).DisplayName, Is.EqualTo("Example Plugin"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(1).PluginId, Is.EqualTo(1));
            Assert.That(project.ProjectPlugins.ElementAt(1).Url, Is.EqualTo("http://another-example.com"));
            Assert.That(project.ProjectPlugins.ElementAt(1).DisplayName, Is.EqualTo("Another example Plugin"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(2).PluginId, Is.EqualTo(3));
            Assert.That(project.ProjectPlugins.ElementAt(2).Url, Is.EqualTo("http://example2.com"));
            Assert.That(project.ProjectPlugins.ElementAt(2).DisplayName, Is.EqualTo("Example 2 Plugin"));
        });
    }
}
