using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class DeleteProjectCommandHandlerTest
{
    private DeleteProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new DeleteProjectCommandHandler(_mockProjectRepo.Object);
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
