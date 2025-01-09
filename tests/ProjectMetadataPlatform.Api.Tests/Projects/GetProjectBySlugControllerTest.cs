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
public class GetProjectBySlugControllerTest
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
    public async Task GetProjectBySlug_NonexistentProject_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "test"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);
        var result = await _controller.Get("test");
        Assert.That(result, Is.Not.Null);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Get("test");
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
            Department = "Security",
            OfferId = "1023",
            Company = "Charlies Schokoladenfabrik",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH
        };
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "metadataplatform"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);
        _mediator.Setup(m => m.Send(It.Is<GetProjectQuery>(q => q.Id == 50), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<GetProjectResponse> result = await _controller.Get("metadataplatform");

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
            Assert.That(project.OfferId, Is.EqualTo("1023"));
            Assert.That(project.Company, Is.EqualTo("Charlies Schokoladenfabrik"));
            Assert.That(project.CompanyState, Is.EqualTo("EXTERNAL"));
            Assert.That(project.IsmsLevel, Is.EqualTo("VERY_HIGH"));
        });
    }
}