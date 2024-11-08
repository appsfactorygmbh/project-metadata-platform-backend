using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Plugins.Models;
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
            "Example Client", [new UpdateProjectPluginRequest("Url", "PluginName", 3)]);
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator => mediator.Send(It.Is<CreateProjectCommand>(command =>
                command.Plugins.Count == 1 && command.ProjectName == "Example Project" && command.BusinessUnit == "Example Business Unit" &&
                command.TeamNumber == 1 && command.Department == "Example Department" && command.ClientName == "Example Client" &&
                command.Plugins.Single().PluginId == 3 && command.Plugins.Single().Url == "Url" && command.Plugins.Single().DisplayName == "PluginName"),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task CreateProjectWithNullProjectPluginList()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest("Example Project", "Example Business Unit", 1, "Example Department",
            "Example Client");
        await _controller.Put(request);
        _mediator.Verify(mediator => mediator.Send(It.Is<CreateProjectCommand>(command =>
                command.Plugins.Count == 0 && command.ProjectName == "Example Project" && command.BusinessUnit == "Example Business Unit" &&
                command.TeamNumber == 1 && command.Department == "Example Department" && command.ClientName == "Example Client"),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task UpdateProjectWithNullProjectPluginList()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest("Example Project", "Example Business Unit", 1, "Example Department",
            "Example Client");
        await _controller.Put(request, 1);
        _mediator.Verify(mediator => mediator.Send(It.Is<UpdateProjectCommand>(command =>
                command.Plugins.Count == 0 && command.ProjectName == "Example Project" && command.BusinessUnit == "Example Business Unit" &&
                command.TeamNumber == 1 && command.Department == "Example Department" && command.ClientName == "Example Client"),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task CreateProject_BadRequestTest()
    {
        var request = new CreateProjectRequest("", " ", 1, "", "");
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateProject_MediatorThrowsInvalidOperationExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("An error message"));
        var request = new CreateProjectRequest("p", "b", 1, "d", "c");
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("An error message"));
    }

    [Test]
    public async Task CreateProject_MediatorThrowsOtherExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var request = new CreateProjectRequest("p", "b", 1, "d", "c");
        ActionResult<CreateProjectResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task ChangeProjectDataControllerTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest("Example Project",
            "Example Business Unit",
            1,
            "Example Department",
            "Example Client",
            [new UpdateProjectPluginRequest("Url", "PluginName", 3)]);
        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);

        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator => mediator.Send(It.Is<UpdateProjectCommand>(command =>
                command.Plugins.Count == 1 && command.ProjectName == "Example Project" && command.BusinessUnit == "Example Business Unit" &&
                command.TeamNumber == 1 && command.Department == "Example Department" && command.ClientName == "Example Client" &&
                command.Plugins.Single().PluginId == 3 && command.Plugins.Single().Url == "Url" &&
                command.Plugins.Single().DisplayName == "PluginName" && command.Plugins.Single().ProjectId == 1),
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task UpdateProject_IsArchivedFlag_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var request = new CreateProjectRequest(
            "Example Project",
            "Example Business Unit",
            1,
            "Example Department",
            "Example Client",
            [new UpdateProjectPluginRequest("Url", "PluginName", 3)],
            true
        );

        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);
        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator => mediator.Send(It.Is<UpdateProjectCommand>(command =>
                command.Plugins.Count == 1 &&
                command.ProjectName == "Example Project" &&
                command.BusinessUnit == "Example Business Unit" &&
                command.TeamNumber == 1 &&
                command.Department == "Example Department" &&
                command.ClientName == "Example Client" &&
                command.Plugins.Single().PluginId == 3 &&
                command.Plugins.Single().Url == "Url" &&
                command.Plugins.Single().DisplayName == "PluginName" &&
                command.Plugins.Single().ProjectId == 1 &&
                command.IsArchived == true
        ), It.IsAny<CancellationToken>()));
    }
}
