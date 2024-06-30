using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class PatchGlobalPluginCommandHandlerTest
{
    
    private PatchGlobalPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _handler = new PatchGlobalPluginCommandHandler(_mockPluginRepo.Object);
    }

    [Test]
    public async Task PatchGlobalPlugin_Test()
    {
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = false };
        var newPlugin = new Plugin { Id = 42, PluginName = "Mercury Atlas", IsArchived = false };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        var result = await _handler.Handle(new PatchGlobalPluginCommand(42, "Mercury Atlas"), It.IsAny<CancellationToken>());
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.EqualTo(newPlugin.IsArchived));
            Assert.That(result.PluginName, Is.EqualTo(newPlugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(newPlugin.Id));
        });
    }
    
    [Test]
    public async Task PatchGlobalPlugin_ChangeNothing_Test()
    {
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = false };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        var result = await _handler.Handle(new PatchGlobalPluginCommand(42), It.IsAny<CancellationToken>());
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));
        });
    }
    
    [Test]
    public async Task PatchGlobalPlugin_NotFound_Test()
    {
        Assert.ThrowsAsync<InvalidOperationException>(PatchGlobalPlugin_NotFound_TestBody);
    }

    private async Task PatchGlobalPlugin_NotFound_TestBody()
    {
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(43)).ThrowsAsync(new InvalidOperationException());

        await _handler.Handle(new PatchGlobalPluginCommand(43, "Mercury Atlas"), It.IsAny<CancellationToken>());
    }
}
