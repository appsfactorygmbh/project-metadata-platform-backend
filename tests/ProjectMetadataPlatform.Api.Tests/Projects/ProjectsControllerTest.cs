using System.Collections;
using System.Collections.Generic;
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
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
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
        var projectsResponseContent = new List<Project>()
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
        var result = await _controller.Get("R");

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
    
     [Test]
    public async Task GetAllPlugins_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProjectPlugins>());
        var result = await _controller.GetPlugins(0);


        Assert.That(result, Is.Not.Null);
        var value = result.Result as OkObjectResult;
        Assert.IsEmpty((IEnumerable)value.Value);

    }

    [Test]
    public async Task GetAllPluginsToId()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var projcet = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new ProjectPlugins{ ProjectId = 1, PluginId = 1, Plugin = plugin,Project = projcet,DisplayName = "Gitlab", Url ="Plugin1.com"},
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        var result = await _controller.GetPlugins(0);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetPluginResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();
        Assert.That(resultValue, Has.Count.EqualTo(1));

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.Url, Is.EqualTo("Plugin1.com"));
            Assert.That(resultObj.PluginName, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.DisplayName, Is.EqualTo("Gitlab"));
        });

    }

    [Test]
    public async Task DisplayNameNullCheckTest()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var projcet = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new ProjectPlugins{ ProjectId = 1, PluginId = 1, Plugin = plugin,Project = projcet, Url ="Plugin1.com"},
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        var result = await _controller.GetPlugins(0);
        var okResult = result.Result as OkObjectResult;
        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.Url, Is.EqualTo("Plugin1.com"));
            Assert.That(resultObj.PluginName, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.DisplayName, Is.EqualTo("plugin 1"));
        });

    }
}
