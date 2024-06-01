using System.Collections;
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
    private ProjectsController _controller;
    private Mock<IMediator> _mediator;
    
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }

    [Test]
    public async Task GetAllProjects_EmptyResponseList_Test()
    {
        // prepare
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        
        // act
        var result = await _controller.Get();
        
        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);
        
        var getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[] ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(0));
    }
    
    [Test]
    public async Task GetAllProjectsTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>
        {
            new ("Regen", "Nasa", "BuWeather", 42)
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(projectsResponseContent);
        
        // act
        var result = await _controller.Get();
        
        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);
        
        var getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[] ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        var project = getProjectsResponseArray.First();
        Assert.That(project.ProjectName, Is.EqualTo("Regen"));
        Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(project.TeamNumber, Is.EqualTo(42));
    }
}