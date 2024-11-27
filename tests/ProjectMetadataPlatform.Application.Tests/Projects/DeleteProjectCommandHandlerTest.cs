using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class DeleteProjectCommandHandlerTest
{
    private DeleteProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteProjectCommandHandler(_mockProjectRepo.Object, _mockLogRepo.Object, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task DeleteProject_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            ClientName = "Metatron",
            BusinessUnit = "666",
            Department = "Silent Hill",
            TeamNumber = 3,
            IsArchived = true
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);
        _mockProjectRepo.Setup(m => m.DeleteProjectAsync(It.IsAny<Project>())).ReturnsAsync(project);

        var result = await _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>());

        Assert.That(project, Is.EqualTo(result));

        _mockLogRepo.Verify(m => m.AddProjectLogForCurrentUser(It.IsAny<Project>(), Action.REMOVED_PROJECT,It.IsAny<List<LogChange>>()), Times.Once);
    }

    [Test]
    public void DeleteProject_ThrowsArgumentException_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            ClientName = "Metatron",
            BusinessUnit = "666",
            Department = "Silent Hill",
            TeamNumber = 3,
            IsArchived = false
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);

        var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>()));
        Assert.That(ex.Message, Is.EqualTo("Project is not archived."));
    }

    [Test]
    public void DeleteProject_NotFound_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync((Project?)null);

        var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>()));
        Assert.That(ex.Message, Is.EqualTo("Project not found."));
    }

}
