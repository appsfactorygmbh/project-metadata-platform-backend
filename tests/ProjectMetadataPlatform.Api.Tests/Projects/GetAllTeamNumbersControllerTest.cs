using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class GetAllTeamNumbersControllerTest
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
    public async Task GetAllTeamNumbersTest()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 50,
                ProjectName = "MetaDataPlatform",
                ClientName = "Appsfactory",
                BusinessUnit = "Health",
                TeamNumber = 42,
                Department = "Security"
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43
            },
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        var result = await _controller.GetAllTeamNumbers();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var response = (okResult.Value, Is.InstanceOf<IEnumerable<int>>());
        Assert.That(response.Value, Is.EquivalentTo(new[] { 42, 43 }));
    }

    [Test]
    public async Task GetAllTeamNumbers_EmptyList_Test()
    {
        var projectsResponseContent = new List<Project>();
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        var result = await _controller.GetAllTeamNumbers();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var response = (okResult.Value, Is.InstanceOf<IEnumerable<int>>());
        Assert.That(response.Value, Is.Empty);
    }


}
