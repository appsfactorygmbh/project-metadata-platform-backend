using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using ProjectMetadataPlatform.Api.Plugins;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Plugins;

public class Tests
{
    private PluginsController _controller;
    private Mock<IMediator> _mediator;
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new PluginsController(_mediator.Object);
    }

    [Test]
    public async Task GetAllPlugins_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProjectPlugins>());
        var result = await _controller.Get(0);


        Assert.IsNotNull(result);
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
        var result = await _controller.Get(0);

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
        var result = await _controller.Get(0);
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
    public async Task CreatePlugin_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var request = new CreatePluginRequest("Solid Rocket Booster");

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreatePluginResponse>());

        var pluginResponse = createdResult.Value as CreatePluginResponse;
        Assert.That(pluginResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(pluginResponse.Id, Is.EqualTo(42));
            Assert.That(createdResult.Location, Is.EqualTo("/Plugins/42"));
        });
    }

    [Test]
    public async Task CreatePlugin_WithError_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException());

        var request = new CreatePluginRequest("Drogue chute");

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());
        var statusResult = result.Result as StatusCodeResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task CreatePlugin_EmptyName_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException());

        var request = new CreatePluginRequest("");

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task CreatePlugin_WhiteSpacesName_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException());

        var request = new CreatePluginRequest("         ");

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task GetGlobalPlugins_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Plugin>());
        var result = await _controller.Get();


        Assert.IsNotNull(result);
        var value = result.Result as OkObjectResult;
        
        Assert.That((IEnumerable) value.Value, Is.Empty);

    }

    [Test]
    public async Task GetGlobalPlugins_Test()
    {
        
        var plugin = new Plugin { Id = 1, PluginName = "plugin 1", IsArchived = false };
        List<Plugin> pluginlist = new List<Plugin> { plugin };

        _mediator.Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pluginlist);
        var result = await _controller.Get();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetGlobalPluginsResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetGlobalPluginsResponse>)!.ToList();
        Assert.That(resultValue, Has.Count.EqualTo(1));

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.pluginName, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.Id, Is.EqualTo(1));
            Assert.That(resultObj.isArchived, Is.False);
            Assert.That(resultObj.keys, Is.EqualTo(new string[] { }));
        });
    }
}
