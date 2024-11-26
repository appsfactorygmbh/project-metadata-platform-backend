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
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class UpdateProjectCommandHandlerTest
{
    private UpdateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepository;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepository = new Mock<ILogRepository>();
        _handler = new UpdateProjectCommandHandler(_mockProjectRepo.Object, _mockPluginRepo.Object,_mockLogRepository.Object, _mockUnitOfWork.Object);
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

        var result = await _handler.Handle(new UpdateProjectCommand(exampleProject.ProjectName, exampleProject.BusinessUnit, exampleProject.TeamNumber, exampleProject.Department, exampleProject.ClientName, exampleProject.Id, projectPluginList, false), It.IsAny<CancellationToken>());
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
                    projectPluginList,
                    exampleProject.IsArchived),
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
                    projectPluginList,
                    exampleProject.IsArchived),
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
            new List<ProjectPlugins>(),
            false);

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
            ]
            ,false);

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

    [Test]
    public async Task UpdateProject_IsArchivedFlag_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            ClientName = "DB",
            BusinessUnit = "Unit 1",
            TeamNumber = 1,
            Department = "Department 1",
            ProjectPlugins = [],
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand("DB App",
            "Unit 2",
            2,
            "Department 2",
            "Deutsche Bahn",
            1,
            new List<ProjectPlugins>(),
            true);

        _mockProjectRepo.Setup(repository => repository.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());
        Assert.That(project.IsArchived, Is.True);
    }

    [Test]
    public async Task LogsChanges_WhenProjectPropertiesAreUpdated()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Old Project Name",
            BusinessUnit = "Old Unit",
            TeamNumber = 1,
            Department = "Old Department",
            ClientName = "Old Client",
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand(
            "New Project Name",
            "New Unit",
            2,
            "New Department",
            "New Client",
            1,
            new List<ProjectPlugins>(),
            false
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            project,
            Action.UPDATED_PROJECT,
            It.Is<List<LogChange>>(changes =>
                changes.Count == 5 &&
                changes.Any(change => change.Property == "ProjectName" && change.OldValue == "Old Project Name" && change.NewValue == "New Project Name") &&
                changes.Any(change => change.Property == "BusinessUnit" && change.OldValue == "Old Unit" && change.NewValue == "New Unit") &&
                changes.Any(change => change.Property == "TeamNumber" && change.OldValue == "1" && change.NewValue == "2") &&
                changes.Any(change => change.Property == "Department" && change.OldValue == "Old Department" && change.NewValue == "New Department") &&
                changes.Any(change => change.Property == "ClientName" && change.OldValue == "Old Client" && change.NewValue == "New Client")
            )
        ), Times.Once);
    }

    [Test]
    public async Task NoLogging_WhenNoPropertiesAreChanged()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "No Change Project",
            ClientName = "Client A",
            BusinessUnit = "Business Unit A",
            TeamNumber = 5,
            Department = "Department A",
            ProjectPlugins = new List<ProjectPlugins>(),
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand(
            project.ProjectName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department,
            project.ClientName,
            project.Id,
            project.ProjectPlugins.ToList(),
            project.IsArchived
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            It.IsAny<Project>(),
            It.IsAny<Action>(),
            It.IsAny<List<LogChange>>()
        ), Times.Never);
    }

    [Test]
    public async Task LogsOnlyChangedProperties()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Partial Update",
            ClientName = "Client A",
            BusinessUnit = "Unit 1",
            TeamNumber = 5,
            Department = "Department A",
            ProjectPlugins = new List<ProjectPlugins>(),
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand(
            "Partial Update",
            "Updated Unit",
            5,
            "Department A",
            "Updated Client",
            1,
            project.ProjectPlugins.ToList(),
            false
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            project,
            Action.UPDATED_PROJECT,
            It.Is<List<LogChange>>(changes =>
                changes.Count == 2 &&
                changes.Any(change => change.Property == "BusinessUnit" && change.OldValue == "Unit 1" && change.NewValue == "Updated Unit") &&
                changes.Any(change => change.Property == "ClientName" && change.OldValue == "Client A" && change.NewValue == "Updated Client")
            )
        ), Times.Once);
    }

    [Test]
    public async Task LogsChanges_HandlesLogRepositoryException()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Project With Exception",
            ClientName = "Client C",
            BusinessUnit = "Unit 3",
            TeamNumber = 4,
            Department = "Department C",
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand(
            "New Project Name",
            "New Unit",
            5,
            "New Department",
            "New Client",
            project.Id,
            new List<ProjectPlugins>(),
            false
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        _mockLogRepository.Setup(logRepo => logRepo.AddLogForCurrentUser(
            It.IsAny<Project>(),
            It.IsAny<Action>(),
            It.IsAny<List<LogChange>>()
        )).Throws(new Exception("Logging error"));

        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _handler.Handle(updateCommand, CancellationToken.None)
        );
        Assert.That(exception.Message, Is.EqualTo("Logging error"));

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            It.IsAny<Project>(),
            It.IsAny<Action>(),
            It.IsAny<List<LogChange>>()
        ), Times.Once);
    }

    [Test]
    public async Task ArchivesProject_WhenIsArchivedFlagIsTrue()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            ClientName = "Test Client",
            BusinessUnit = "Test Unit",
            TeamNumber = 1,
            Department = "Test Department",
            ProjectPlugins = new List<ProjectPlugins>(),
            IsArchived = false
        };

        var updateCommand = new UpdateProjectCommand(
            project.ProjectName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department,
            project.ClientName,
            project.Id,
            new List<ProjectPlugins>(),
            true
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.True);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            project,
            Action.ARCHIVED_PROJECT,  // Expect Action.ARCHIVED_PROJECT since the project was archived
            It.Is<List<LogChange>>(changes =>
                changes.Count == 1 &&
                changes.Any(change => change.Property == "IsArchived" && change.OldValue == "False" && change.NewValue == "True")
            )
        ), Times.Once);
        _mockLogRepository.VerifyNoOtherCalls();
    }



    [Test]
    public async Task UnArchivesProject_WhenIsArchivedFlagIsFalse()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Archived Project",
            ClientName = "Test Client",
            BusinessUnit = "Test Unit",
            TeamNumber = 1,
            Department = "Test Department",
            ProjectPlugins = new List<ProjectPlugins>(),
            IsArchived = true
        };

        var updateCommand = new UpdateProjectCommand(
            project.ProjectName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department,
            project.ClientName,
            project.Id,
            new List<ProjectPlugins>(),
            false
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.False);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            project,
            Action.UNARCHIVED_PROJECT,
            It.Is<List<LogChange>>(changes =>
                changes.Count == 1 &&
                changes.Any(change => change.Property == "IsArchived" && change.OldValue == "True" && change.NewValue == "False")
            )
        ), Times.Once);
        _mockLogRepository.VerifyNoOtherCalls();
    }


    [Test]
    public async Task DoesNotLogWhenIsArchivedStatusDoesNotChange()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            ClientName = "Test Client",
            BusinessUnit = "Test Unit",
            TeamNumber = 1,
            Department = "Test Department",
            ProjectPlugins = new List<ProjectPlugins>(),
            IsArchived = true
        };

        var updateCommand = new UpdateProjectCommand(
            project.ProjectName,
            project.BusinessUnit,
            project.TeamNumber,
            project.Department,
            project.ClientName,
            project.Id,
            new List<ProjectPlugins>(),
            true
        );

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(project.Id)).ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.True);

        _mockLogRepository.Verify(logRepo => logRepo.AddLogForCurrentUser(
            project,
            Action.UPDATED_PROJECT,
            It.IsAny<List<LogChange>>()
        ), Times.Never);
    }

}
