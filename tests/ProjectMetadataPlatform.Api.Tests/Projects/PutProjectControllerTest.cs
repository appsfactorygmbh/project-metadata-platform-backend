using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;


[TestFixture]
public class PutProjectControllerTest
{
    private ProjectsController _controller;
    private Mock<IMediator> _mediator;
    
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }

    [Test]
    public async Task CreateProject_Test()
    {
        //prepare
        var exampleProject = new Project
        {
            
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client"
        };
        _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleProject);
        var request = new CreateProjectRequest( "Example Project", "Example Business Unit", 1, "Example Department", "Example Client");
        ActionResult<Project> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;
        
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());
        
        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        
        Assert.That(createdResult.Location, Is.Not.Null);
    }

    [Test]
    public async Task CreateProject_BadRequestTest()
    {
     _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new IOException());
     var request = new CreateProjectRequest( "", " ", 1, "", "");
     ActionResult<Project> result = await _controller.Put(request);
     Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
}   
