using System;
using System.Collections;
using System.Collections.Generic;
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
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Microsoft.AspNetCore.Http;

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
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get(null, null);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetProjectsResponse>>());

        var getProjectsResponseArray = (okResult.Value as IEnumerable<GetProjectsResponse>)?.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

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
                Slug = "regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get(null, null);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetProjectsResponse>>());

        var getProjectsResponseArray = (okResult.Value as IEnumerable<GetProjectsResponse>)?.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        GetProjectsResponse project = getProjectsResponseArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.Slug, Is.EqualTo("regen"));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(project.TeamNumber, Is.EqualTo(42));
        });
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
                Slug = "regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllProjectsQuery>(x => x.Search == "R"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        ActionResult<IEnumerable<GetProjectsResponse>> result = await _controller.Get(null, "R");

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetProjectsResponse>>());

        var getProjectsResponseArray = (okResult.Value as IEnumerable<GetProjectsResponse>)?.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        GetProjectsResponse project = getProjectsResponseArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.Slug, Is.EqualTo("regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(project.TeamNumber, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task GetAllProjects_MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Get(null, "search");
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetAllPlugins_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProjectPlugins>());
        var result = await _controller.GetPlugins(0);


        Assert.That(result, Is.Not.Null);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable>());
            Assert.That((IEnumerable)okResult.Value!, Is.Empty);
        });
    }

    [Test]
    public async Task MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.GetPlugins(1);
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetAllPluginsToId()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var projcet = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", Slug = "project_1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin,Project = projcet,DisplayName = "Gitlab", Url ="Plugin1.com"},
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
    public async Task GetPluginsForProjectByProjectSlug_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 5), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Bor"), It.IsAny<CancellationToken>())).ReturnsAsync(5);

        var result = await _controller.GetPluginsBySlug("Bor");

        Assert.That(result, Is.Not.Null);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable>());
            Assert.That((IEnumerable)okResult.Value!, Is.Empty);
        });
        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Bor"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetPluginsForProjectByProjectSlug_SlugNotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Mendelev"), It.IsAny<CancellationToken>())).ReturnsAsync((int?)null);

        var result = await _controller.GetPluginsBySlug("Mendelev");

        Assert.That(result, Is.Not.Null);
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("Project with Slug Mendelev not found."));

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Mendelev"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 101), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetPluginsForProjectByProjectSlug_MediatorThrowsExceptionWhenRequestingPlugins_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Meitner"), It.IsAny<CancellationToken>())).ReturnsAsync(109);
        _mediator.Setup(mediator => mediator.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 109), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.GetPluginsBySlug("Meitner");
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Meitner"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 109), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetPluginsForProjectByProjectSlug_MediatorThrowsExceptionWhenRequestingIdBySlug_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Curie"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));

        var result = await _controller.GetPluginsBySlug("Curie");
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Curie"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetPluginsForProjectByProjectSlug()
    {
        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var project = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", Slug = "project_1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin, Project = project, DisplayName = "Gitlab", Url = "Plugin1.com"},
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 100), It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Fermi"), It.IsAny<CancellationToken>())).ReturnsAsync(100);

        var result = await _controller.GetPluginsBySlug("Fermi");

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

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Fermi"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 100), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetPluginsForProjectByProjectSlug_PluginDisplayNameNullIsReplacedByPluginName_Test()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var project = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", Slug = "project_1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin, Project = project, Url = "Plugin1.com"},
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 111), It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Röntgen"), It.IsAny<CancellationToken>())).ReturnsAsync(111);
        var result = await _controller.GetPluginsBySlug("Röntgen");
        var okResult = result.Result as OkObjectResult;
        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.Url, Is.EqualTo("Plugin1.com"));
            Assert.That(resultObj.PluginName, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.DisplayName, Is.EqualTo("plugin 1"));
        });
        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(r => r.Slug == "Röntgen"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllPluginsForProjectIdQuery>(r => r.Id == 111), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetPluginsByProject_PluginDisplayNameNullIsReplacedByPluginName_Test()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var projcet = new Project { Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", Slug = "project_1", TeamNumber = 1 };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin,Project = projcet, Url ="Plugin1.com"},
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

    [Test]
    public async Task GetProjectByFiltersAndSearchTest()
    {
        var search = "Hea";
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Metatron",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            true

        );

        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<Project>
            {
                new()
                {
                    Id = 1,
                    ProjectName = "Heather",
                    Slug = "heather",
                    BusinessUnit = "666",
                    ClientName = "Metatron",
                    Department = "Mars",
                    TeamNumber = 42,
                    IsArchived = true
                }
            });

        var result = await _controller.Get(filters, search);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        var response = (okResult.Value as IEnumerable<GetProjectsResponse>)?.ToList();
        Assert.That(response, Is.Not.Null);

        Assert.Multiple((() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Has.Count.EqualTo(1));
            Assert.That(response.ToArray()[0].Id, Is.EqualTo(1));
            Assert.That(response.ToArray()[0].ProjectName, Is.EqualTo("Heather"));
            Assert.That(response.ToArray()[0].Slug, Is.EqualTo("heather"));
            Assert.That(response.ToArray()[0].BusinessUnit, Is.EqualTo("666"));
            Assert.That(response.ToArray()[0].ClientName, Is.EqualTo("Metatron"));
            Assert.That(response.ToArray()[0].TeamNumber, Is.EqualTo(42));
            Assert.That(response.ToArray()[0].IsArchived, Is.EqualTo(true));
        }));
    }

    [Test]
    public async Task GetProjectByFiltersAndSearchTest_NoMatch()
    {
        var search = "Hea";
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Gilgamesch",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            false
        );

        _mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), CancellationToken.None))
            .ReturnsAsync([]);

        var result = await _controller.Get(filters, search);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        var response = (okResult.Value as IEnumerable<GetProjectsResponse>)?.ToArray();
        Assert.Multiple((() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.Empty);
        }));
    }

    [Test]
    public async Task GetUnarchivedPlugins_ReturnsOkWithPlugins()
    {
        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin, DisplayName = "Gitlab", Url = "Plugin1.com" }
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);

        var result = await _controller.GetUnarchivedPlugins(1);

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
    public async Task GetUnarchivedPlugins_WhenMediatorThrows_Returns500()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetUnarchivedPlugins(1);

        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var statusCodeResult = result.Result as StatusCodeResult;
        Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task GetUnarchivedPlugins_WhenNoPlugins_ReturnsOkWithEmptyList()
    {
        var responseContent = new List<ProjectPlugins>(); // No plugins
        _mediator.Setup(m => m.Send(It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);

        var result = await _controller.GetUnarchivedPlugins(1);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetPluginResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();
        Assert.That(resultValue, Is.Empty);
    }

    [Test]
    public async Task GetUnarchivedPlugins_WhenPluginIsNull_SkipsNullValues()
    {
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = null, DisplayName = "Gitlab", Url = "Plugin1.com" }
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);

        var result = await _controller.GetUnarchivedPlugins(1);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();

        Assert.That(resultValue, Is.Empty);
    }

    [Test]
    public async Task GetUnarchivedPlugins_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        var nonExistentProjectId = 999;

        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == nonExistentProjectId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException($"Project with Id {nonExistentProjectId} not found."));

        var result = await _controller.GetUnarchivedPlugins(nonExistentProjectId);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.EqualTo($"Project with Id {nonExistentProjectId} not found."));
    }

    [Test]
    public async Task GetUnarchivedPluginsById_ReturnsOkWithPlugins()
    {
        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = plugin, DisplayName = "Gitlab", Url = "Plugin1.com" }
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _controller.GetUnarchivedPluginsBySlug("project_1");

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

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetUnarchivedPluginsBySlug_WhenMediatorThrows_Returns500()
    {
        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetUnarchivedPluginsBySlug("project_1");

        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var statusCodeResult = result.Result as StatusCodeResult;
        Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetUnarchivedPluginsBySlug_WhenNoPlugins_ReturnsOkWithEmptyList()
    {
        var responseContent = new List<ProjectPlugins>(); // No plugins
        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _controller.GetUnarchivedPluginsBySlug("project_1");

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetPluginResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();
        Assert.That(resultValue, Is.Empty);
    }

    [Test]
    public async Task GetUnarchivedPluginsBySlug_WhenPluginIsNull_SkipsNullValues()
    {
        var responseContent = new List<ProjectPlugins>
        {
            new() { ProjectId = 1, PluginId = 1, Plugin = null, DisplayName = "Gitlab", Url = "Plugin1.com" }
        };

        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseContent);
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _controller.GetUnarchivedPluginsBySlug("project_1");

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();

        Assert.That(resultValue, Is.Empty);

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "project_1"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetUnarchivedPluginsBySlug_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        var nonExistentProjectId = 999;

        _mediator.Setup(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == nonExistentProjectId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException($"Project with Id {nonExistentProjectId} not found."));
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "non_existent_project"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        var result = await _controller.GetUnarchivedPluginsBySlug("non_existent_project");
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.EqualTo($"Project with Slug non_existent_project not found."));

        _mediator.Verify(m => m.Send(It.Is<GetProjectIdBySlugQuery>(x => x.Slug == "non_existent_project"), It.IsAny<CancellationToken>()), Times.Once);;
        _mediator.Verify(m => m.Send(It.Is<GetAllUnarchivedPluginsForProjectIdQuery>(x => x.Id == nonExistentProjectId), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteProject_ReturnsOk()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            BusinessUnit = "666",
            Department = "Silent Hill",
            TeamNumber = 3,
            IsArchived = true
        };

        _mediator.Setup(m => m.Send(It.Is<DeleteProjectCommand>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteProject_WhenProjectIsNotArchived_ReturnsBadRequest()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            BusinessUnit = "666",
            Department = "Silent Hill",
            TeamNumber = 3,
            IsArchived = false
        };

        _mediator
            .Setup(m => m.Send(It.Is<DeleteProjectCommand>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Project is not archived."));

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult!.Value, Is.EqualTo("Project is not archived."));
    }

    [Test]
    public async Task DeleteProject_WhenProjectDoesNotExist_ReturnsBadRequest()
    {
        _mediator
            .Setup(m => m.Send(It.Is<DeleteProjectCommand>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Project not found."));

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult!.Value, Is.EqualTo("Project not found."));
    }

    [Test]
    public async Task DeleteProject_InternalServerError()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<StatusCodeResult>());

        var statusCodeResult = result as StatusCodeResult;
        Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task DeleteProjectBySlug_ReturnsOk()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            BusinessUnit = "666",
            Department = "Silent Hill",
            TeamNumber = 3,
            IsArchived = true
        };

        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "heather"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mediator.Setup(m => m.Send(It.Is<DeleteProjectCommand>(x => x.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _controller.Delete("heather");

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }



    [Test]
    public async Task DeleteProjectBySlug_WhenProjectDoesNotExist()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "test"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);


        var result = await _controller.Delete("test");

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundRequestResult = result as NotFoundObjectResult;
        Assert.That(notFoundRequestResult!.Value, Is.EqualTo("Project with Slug test not found."));
    }

    [Test]
    public async Task DeleteProjectBySlug_InternalServerError()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Delete("test");
        Assert.That(result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

}
