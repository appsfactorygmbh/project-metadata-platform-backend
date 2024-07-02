using System;
using NUnit.Framework;
using Moq;
using System.Threading;
using System.Threading.Tasks;
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
        
        var result = await _handler.Handle(new DeleteGlobalPluginCommand(42), It.IsAny<CancellationToken>());
        Assert.That(result.IsArchived, Is.EqualTo(true));
    }

    [Test]
    public async Task DeleteGlobalPluginNull_Test()
    {
       Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(new DeleteGlobalPluginCommand(0), It.IsAny<CancellationToken>()));
    }
    [Test]
    public async Task DeleteGlobalPluginNullPointerException_Test()
    {
        _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync((Plugin)null!);
        var result = await _handler.Handle(new DeleteGlobalPluginCommand(42), CancellationToken.None);
        Assert.That(result, Is.Null);
    }
    
    
}
