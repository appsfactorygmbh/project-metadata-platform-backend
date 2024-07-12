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

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

public class GetAllTeamNumberQueryHandlerTest
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
        IEnumerable<int> projectsResponseContent = new List<int>()
        {
            42,
            43
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllTeamNumbersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        var result = await _controller.GetAllTeamNumbers();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        var response = (okResult.Value, Is.InstanceOf<IEnumerable<int>>());
        Assert.That(response.Value, Is.EquivalentTo(projectsResponseContent));
    }
}
