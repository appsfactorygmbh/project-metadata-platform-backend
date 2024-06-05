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
        var plugins = new List<Plugin>
        {
            new Plugin
            {
                Id = 1,
                PluginName = "Plugin 1",
                Url = "http://plugin1.com",
            },
            new Plugin
            {
                Id = 1,
                PluginName = "Plugin 2",
                Url = "http://plugin2.com",
            }
        };

        _pluginRepositoryMock.Setup(r => r.GetAllPluginsForProjectIdAsync(It.IsAny<int>())).ReturnsAsync(plugins);

        var query = new GetAllPluginsForProjectIdQuery(It.IsAny<int>());
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<List<Plugin>>());
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[0].Url, Is.EqualTo("http://plugin1.com"));
            Assert.That(result[1].PluginName, Is.EqualTo("Plugin 2"));
            Assert.That(result[1].Url, Is.EqualTo("http://plugin2.com"));
        });

    }
}