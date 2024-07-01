using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

public class UpdateProjectCommandHandlerTest
{
    private UpdateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new UpdateProjectCommandHandler(_mockProjectRepo.Object);
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
        
        _mockProjectRepo.Setup(m => m.UpdateProject(It.IsAny<Project>(),It.IsAny<List<ProjectPlugins>>()))
            .Callback((Project p, List<ProjectPlugins> plugins) => p.Id = 1)
            .Returns(Task.CompletedTask);

        _mockProjectRepo.Setup(m => m.CheckProjectExists(1))
            .ReturnsAsync(true);
        
        var result = await _handler.Handle(new UpdateProjectCommand(exampleProject.ProjectName, exampleProject.BusinessUnit, exampleProject.TeamNumber, exampleProject.Department, exampleProject.ClientName,exampleProject.Id,projectPluginList), It.IsAny<CancellationToken>());
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
        
        _mockProjectRepo.Setup(m => m.UpdateProject(It.IsAny<Project>(),It.IsAny<List<ProjectPlugins>>()))
            .Callback((Project p, List<ProjectPlugins> plugins) => p.Id = 1)
            .Returns(Task.CompletedTask);

        _mockProjectRepo.Setup(m => m.CheckProjectExists(1))
            .ReturnsAsync(false);

        var exception =  Assert.ThrowsAsync<InvalidOperationException>(async () =>
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

}
