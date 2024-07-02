using NUnit.Framework;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;
namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class DeletePluginCommandHandlerTest
{
    private DeleteGlobalPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;
    
    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _handler = new DeleteGlobalPluginCommandHandler(_mockPluginRepo.Object);
        
    }
    
    
    [Test]
    public async Task DeleteGlobalPlugin_Test()
    {
        

        var plugin = new Plugin { Id = 42, PluginName = "Flat-Earth", IsArchived = false };
        _mockPluginRepo.Setup(m => m.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        
        await _handler.Handle(new DeleteGlobalPluginCommand(42), It.IsAny<CancellationToken>());
        var result = await _mockPluginRepo.Object.GetPluginByIdAsync(42);
        Assert.That(result.IsArchived, Is.EqualTo(true));
        
        
    }
}
