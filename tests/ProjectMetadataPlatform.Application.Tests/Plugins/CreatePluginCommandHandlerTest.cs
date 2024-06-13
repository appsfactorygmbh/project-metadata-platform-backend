using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class CreatePluginCommandHandlerTest
{
    private CreatePluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _handler = new CreatePluginCommandHandler(_mockPluginRepo.Object);
    }
    
    [Test]
    public async Task CreatePlugin_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Airlock", Id = 13, ProjectPlugins = [] };
        _mockPluginRepo.Setup(m => m.Update(It.IsAny<Plugin>())).ReturnsAsync(examplePlugin);

        var result = await _handler.Handle(new CreatePluginCommand("Airlock"), It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(13));
    }
}
