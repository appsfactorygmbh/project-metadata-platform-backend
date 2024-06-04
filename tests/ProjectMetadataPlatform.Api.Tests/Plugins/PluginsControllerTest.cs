using System.Collections.Generic;
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
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(),It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var result =  await _controller.Get(0);
        
        
        Assert.IsNull(result.Value);
    }

    [Test]
    public async Task GetAllPluginsToId()
    {
        var responseContent = new List<Plugin>
        
        {
            new Plugin{ Id = 1, PluginName = "TestPlugin", Url = "http://test.com"},
            new Plugin{ Id = 2, PluginName = "TestPlugin2", Url = "http://test2.com"}
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(),It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        var result =  await _controller.Get(0);
        
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult!.Value);
        Assert.IsInstanceOf<IEnumerable<GetPluginResponse>>(okResult.Value);
        
        var resultValue = (okResult.Value as IEnumerable<GetPluginResponse>)!.ToList();
        Assert.That(resultValue.Count(), Is.EqualTo(2));
        
        Assert.That(resultValue.First().PluginName, Is.EqualTo("TestPlugin"));
        Assert.That(resultValue.ElementAt(1).PluginName, Is.EqualTo("TestPlugin2"));
        
        Assert.That(resultValue.First().Url, Is.EqualTo("http://test.com"));
        Assert.That(resultValue.ElementAt(1).Url, Is.EqualTo("http://test2.com"));
    }
}