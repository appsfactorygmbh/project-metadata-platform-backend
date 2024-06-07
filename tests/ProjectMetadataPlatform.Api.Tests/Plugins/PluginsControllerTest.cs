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
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(),It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var result =  await _controller.Get(0);
        
        
        Assert.IsNull(result.Value);
        if (result.Value != null)
        {
            Assert.IsNotEmpty(result.Value);
        }
        
    }

    [Test]
    public async Task GetAllPluginsToId()
    {

        var plugin = new Plugin { Id = 1, PluginName = "plugin 1" };
        var projcet = new Project {Id = 1, Department = "department 1", BusinessUnit = "business unit 1", ClientName = "client name 1", ProjectName = "project 1", TeamNumber = 1};
        var responseContent = new List<ProjectPlugins>
        
        {
            new ProjectPlugins{ ProjectId = 1, PluginId = 1, Plugin = plugin,Project = projcet,DisplayName = "Gitlab"},
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllPluginsForProjectIdQuery>(),It.IsAny<CancellationToken>())).ReturnsAsync(responseContent);
        var result =  await _controller.Get(0);
        
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        
        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetPluginResponse>>());
        });
        var resultValue = (okResult?.Value as IEnumerable<GetPluginResponse>)!.ToList();
        Assert.That(resultValue, Has.Count.EqualTo(1));
        
    }
}