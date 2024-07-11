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

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class PutProjectControllerTest
{

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }
    private ProjectsController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public async Task CreateProject_Test()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest("Example Project", "Example Business Unit", 1, "Example Department",
            "Example Client", []);
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.That(projectResponse.Id, Is.EqualTo(1));

        Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
    }

    [Test]
    public async Task CreateProject_BadRequestTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException());
        var request = new CreateProjectRequest("", " ", 1, "", "");
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task ChangeProjectDataControllerTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest("Example Project", "Example Business Unit", 1, "Example Department",
            "Example Client", []);
        var result = await _controller.Put(request, 1);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;
        var projectResponse = createdResult!.Value as CreateProjectResponse;
        Assert.That(projectResponse!.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task ChangeProjectIdNotFoundControllerTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Project does not exist."));
        var request = new CreateProjectRequest("Example Project", "Example Business Unit", 1, "Example Department",
            "Example Client", []);

        var result = await _controller.Put(request, 2);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult!.Value, Is.EqualTo("Project does not exist."));
    }
}
