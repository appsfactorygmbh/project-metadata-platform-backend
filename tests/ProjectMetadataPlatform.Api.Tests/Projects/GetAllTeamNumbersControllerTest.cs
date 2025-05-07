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

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class GetAllTeamNumbersControllerTest
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
    public async Task GetAllTeamNumbersTest()
    {
        IEnumerable<int> projectsResponseContent = new List<int>() { 42, 43 };
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllTeamNumbersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        var result = await _controller.GetAllTeamNumbers();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var response = (okResult.Value, Is.InstanceOf<IEnumerable<int>>());
        Assert.That(response.Value, Is.EquivalentTo(projectsResponseContent));
    }

    [Test]
    public async Task GetAllTeamNumbers_EmptyList_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllTeamNumbersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int>());

        var result = await _controller.GetAllTeamNumbers();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var response = (okResult.Value, Is.InstanceOf<IEnumerable<int>>());
        Assert.That(response.Value, Is.Empty);
    }
}
