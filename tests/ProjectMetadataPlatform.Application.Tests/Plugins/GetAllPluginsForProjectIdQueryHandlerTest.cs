using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class GetAllPluginsForProjectIdQueryHandlerTest
{
    private GetAllPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;
    
    [SetUp]
    public void SetUp()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetAllPluginsForProjectIdQueryHandler(_pluginRepositoryMock.Object);
    }
    
    [Test]
    public async Task HandleGetAllProjectsForProjectIdQueryHandlerTest()
    {
        // Arrange
        var plugins = new List<ProjectPlugins>
        {
            new ProjectPlugins
            {
                PluginId = 1,
                Plugin = null,
                
            },
            
        };

        _pluginRepositoryMock.Setup(r => r.GetAllPluginsForProjectIdAsync(It.IsAny<int>())).ReturnsAsync(plugins);

        var query = new GetAllPluginsForProjectIdQuery(It.IsAny<int>());
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<List<Plugin>>());
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Plugin.PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[1].Plugin.PluginName, Is.EqualTo("Plugin 2"));
        });

    }
}