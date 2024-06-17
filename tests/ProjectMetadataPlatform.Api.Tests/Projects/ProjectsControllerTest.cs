using System.Collections.Generic;
using System.Linq;
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
public class ProjectsControllerTest
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
    public async Task GetAllProjects_EmptyResponseList_Test()
    {
        // prepare
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // act
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get();

        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);

        GetProjectsResponse[] getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[]
                                                         ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task GetAllProjectsTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get();

        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);

        GetProjectsResponse[] getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[]
                                                         ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        GetProjectsResponse project = getProjectsResponseArray.First();
        Assert.That(project.Id, Is.EqualTo(1));
        Assert.That(project.ProjectName, Is.EqualTo("Regen"));
        Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(project.TeamNumber, Is.EqualTo(42));
    }

    [Test]
    public async Task GetProjectBySearchControllerTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 0,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllProjectsQuery>(x => x.Search == "R"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get("R");

        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);

        GetProjectsResponse[] getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[]
                                                         ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        GetProjectsResponse project = getProjectsResponseArray.First();
        Assert.That(project.ProjectName, Is.EqualTo("Regen"));
        Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(project.TeamNumber, Is.EqualTo(42));

    }
}
