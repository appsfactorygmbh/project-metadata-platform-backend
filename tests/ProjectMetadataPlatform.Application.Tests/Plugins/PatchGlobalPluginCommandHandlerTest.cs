using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class PatchGlobalPluginCommandHandlerTest
{

    private PatchGlobalPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PatchGlobalPluginCommandHandler(_mockPluginRepo.Object, _mockLogRepo.Object, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdatePluginName_Test()
    {
        // Arrange
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = false };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        List<LogChange> capturedLogChanges = null;

        _mockLogRepo.Setup(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UPDATED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()))
            .Callback<Plugin, Action, List<LogChange>>((plugin, action, logChanges) =>
                capturedLogChanges = logChanges);

        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new PatchGlobalPluginCommand(42, "Mercury Atlas"), It.IsAny<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PluginName, Is.EqualTo("Mercury Atlas"));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UPDATED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()), Times.Once);

            Assert.That(capturedLogChanges, Is.Not.Null);
            Assert.That(capturedLogChanges.Count, Is.EqualTo(1));
            Assert.That(capturedLogChanges.First().Property, Is.EqualTo(nameof(plugin.PluginName)));
            Assert.That(capturedLogChanges.First().OldValue, Is.EqualTo("Mercury Redstone"));
            Assert.That(capturedLogChanges.First().NewValue, Is.EqualTo("Mercury Atlas"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });
    }

    [Test]
    public async Task PatchGlobalPlugin_ArchivePlugin_Test()
    {
        // Arrange
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = false };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        List<LogChange> capturedLogChanges = null;

        _mockLogRepo.Setup(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.ARCHIVED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()))
            .Callback<Plugin, Action, List<LogChange>>((plugin, action, logChanges) =>
                capturedLogChanges = logChanges);

        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new PatchGlobalPluginCommand(42, null, true), It.IsAny<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.True);
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.ARCHIVED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()), Times.Once);

            Assert.That(capturedLogChanges, Is.Not.Null);
            Assert.That(capturedLogChanges.Count, Is.EqualTo(1));
            Assert.That(capturedLogChanges.First().Property, Is.EqualTo(nameof(plugin.IsArchived)));
            Assert.That(capturedLogChanges.First().OldValue, Is.EqualTo("False"));
            Assert.That(capturedLogChanges.First().NewValue, Is.EqualTo("True"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });
    }





    [Test]
    public async Task PatchGlobalPlugin_ChangeNothing_Test()
    {
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = false };

        // Mock repository setup
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        List<LogChange> capturedLogChanges = null;

        // Mock log repository to capture any log changes
        _mockLogRepo.Setup(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UPDATED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()))
            .Callback<Plugin, Action, List<LogChange>>((_, __, logChanges) => capturedLogChanges = logChanges);

        var result = await _handler.Handle(new PatchGlobalPluginCommand(42), It.IsAny<CancellationToken>());

        // Print the result for debugging
        TestContext.Out.WriteLine($"[Test Debug] Result PluginName: {result.PluginName}, IsArchived: {result.IsArchived}");
        if (capturedLogChanges != null)
        {
            TestContext.Out.WriteLine($"[Test Debug] Captured Log Changes: {string.Join(", ", capturedLogChanges.Select(c => $"{c.Property}: {c.OldValue} -> {c.NewValue}"))}");
        }

        Assert.Multiple(() =>
        {
            // Validate that the plugin hasn't changed
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            // Verify that no log changes were captured (since nothing changed)
            Assert.That(capturedLogChanges, Is.Null, "Expected no log changes to be captured.");
        });
    }

    [Test]
    public async Task PatchGlobalPlugin_UnarchivePlugin_Test()
    {
        // Arrange
        var plugin = new Plugin { Id = 42, PluginName = "Mercury Redstone", IsArchived = true };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo.Setup(repo => repo.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync((Plugin p) => p);

        List<LogChange> capturedLogChanges = null;

        _mockLogRepo.Setup(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UNARCHIVED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()))
            .Callback<Plugin, Action, List<LogChange>>((plugin, action, logChanges) =>
                capturedLogChanges = logChanges);

        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new PatchGlobalPluginCommand(42, null, false), It.IsAny<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.False);
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UNARCHIVED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()), Times.Once);

            Assert.That(capturedLogChanges, Is.Not.Null);
            Assert.That(capturedLogChanges.Count, Is.EqualTo(1));
            Assert.That(capturedLogChanges.First().Property, Is.EqualTo(nameof(plugin.IsArchived)));
            Assert.That(capturedLogChanges.First().OldValue, Is.EqualTo("True"));
            Assert.That(capturedLogChanges.First().NewValue, Is.EqualTo("False"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });
    }


    [Test]
    public async Task PatchGlobalPlugin_NotFound_Test()
    {
        // Simulate a scenario where no plugin is found by ID
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync((Plugin)null);

        List<LogChange> capturedLogChanges = null;

        // Mock log repository to capture log changes (in case of failure)
        _mockLogRepo.Setup(m => m.AddLogForCurrentUser(
                It.IsAny<Plugin>(),
                Action.UPDATED_GLOBAL_PLUGIN,
                It.IsAny<List<LogChange>>()))
            .Callback<Plugin, Action, List<LogChange>>((_, __, logChanges) => capturedLogChanges = logChanges);

        // Validate that an exception is thrown when no plugin is found
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _handler.Handle(new PatchGlobalPluginCommand(42), It.IsAny<CancellationToken>());
        });

        // Verify that no log changes were captured since no valid plugin was found
        Assert.That(capturedLogChanges, Is.Null, "Expected no log changes when plugin not found.");
    }

    private async Task PatchGlobalPlugin_NotFound_TestBody()
    {
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(43)).ThrowsAsync(new InvalidOperationException());

        await _handler.Handle(new PatchGlobalPluginCommand(43, "Mercury Atlas"), It.IsAny<CancellationToken>());
    }
}
