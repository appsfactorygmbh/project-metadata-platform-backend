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
    public async Task CreatePlugin_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var request = new CreatePluginRequest("Solid Rocket Booster", true, new List<string>());

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

        var request = new CreatePluginRequest("Drogue chute", false, new List<string>());

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

        var request = new CreatePluginRequest("", false, new List<string>());

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task DeletePlugin_MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Delete(1);
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task PatchPlugin_MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Patch(1, new PatchGlobalPluginRequest());
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetGlobalPlugins_MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.GetGlobal();
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task CreatePlugin_WhiteSpacesName_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException());

        var request = new CreatePluginRequest("         ", false, new List<string>());

        ActionResult<CreatePluginResponse> result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task UpdateGlobalPlugin_Test()
    {
        var plugin = new Plugin { Id = 1, PluginName = "horn ox", IsArchived = true };
        _mediator.Setup(m => m.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugin);

        var request = new PatchGlobalPluginRequest(null, true);

        ActionResult<GetGlobalPluginResponse> result = await _controller.Patch(1, request);
        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as GetGlobalPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.Name, Is.EqualTo("horn ox"));
            Assert.That(resultValue.IsArchived, Is.EqualTo(true));
            Assert.That(resultValue.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Patch_PluginNotFound_ReturnsNotFound()
    {
        // Arrange
        var pluginId = 1;
        var request = new PatchGlobalPluginRequest(null, true);

        _mediator
            .Setup(m => m.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plugin?)null); // Simulate a null response when plugin is not found

        // Act
        ActionResult<GetGlobalPluginResponse> result = await _controller.Patch(pluginId, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult!.Value, Is.EqualTo("No Plugin with id " + pluginId + " was found."));
    }

    [Test]
    public async Task GetGlobalPlugins_EmptyResponseList_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Plugin>());
        var result = await _controller.GetGlobal();


        Assert.IsNotNull(result);
        var value = result.Result as OkObjectResult;

        Assert.That((IEnumerable)value.Value, Is.Empty);

    }

    [Test]
    public async Task GetGlobalPlugins_Test()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1", IsArchived = false };
        List<Plugin> pluginlist = new List<Plugin> { plugin };

        _mediator.Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pluginlist);
        var result = await _controller.GetGlobal();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetGlobalPluginResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetGlobalPluginResponse>)!.ToList();
        Assert.That(resultValue, Has.Count.EqualTo(1));

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.Name, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.Id, Is.EqualTo(1));
            Assert.That(resultObj.IsArchived, Is.False);
            Assert.That(resultObj.Keys, Is.EqualTo(System.Array.Empty<string>()));
        });
    }

    [Test]
    public async Task DeleteGlobalPlugin_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ActionResult<DeleteGlobalPluginResponse> result = await _controller.Delete(37);


        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as DeleteGlobalPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.PluginId, Is.EqualTo(37));
        });
    }

    [Test]
    public async Task DeleteGlobalPlugin_PluginNotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((bool?)(null));

        ActionResult<DeleteGlobalPluginResponse> result = await _controller.Delete(37);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("No Plugin with id 37 was found."));
    }

    [Test]
    public async Task DeleteGlobalPlugin_InvalidId_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        ActionResult<DeleteGlobalPluginResponse> result = await _controller.Delete(0);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

        var badRequestResult = result.Result as ObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("PluginId can't be 0"));
    }

}
