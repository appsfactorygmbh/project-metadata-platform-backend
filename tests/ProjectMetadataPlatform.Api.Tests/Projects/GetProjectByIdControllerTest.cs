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
public class GetProjectByIdControllerTest
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
    public async Task GetProjectById_NonexistentProject_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);
        ActionResult<GetProjectResponse> result = await _controller.Get(1);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetProjectQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new Project
        {
            Id = 50,
            ProjectName = "MetaDataPlatform",
            Slug = "metadataplatform",
            ClientName = "Appsfactory",
            BusinessUnit = "BusinessUnit",
            TeamNumber = 200,
            Department = "Security"
        };
        _mediator.Setup(m => m.Send(It.Is<GetProjectQuery>(q => q.Id == 50), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<GetProjectResponse> result = await _controller.Get(50);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetProjectResponse>());

        var getProjectResponse = okResult.Value as GetProjectResponse;
        Assert.That(getProjectResponse, Is.Not.Null);

        GetProjectResponse? project = getProjectResponse;
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(50));
            Assert.That(project.ProjectName, Is.EqualTo("MetaDataPlatform"));
            Assert.That(project.Slug, Is.EqualTo("metadataplatform"));
            Assert.That(project.ClientName, Is.EqualTo("Appsfactory"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BusinessUnit"));
            Assert.That(project.TeamNumber, Is.EqualTo(200));
            Assert.That(project.Department, Is.EqualTo("Security"));
        });
    }
}
