using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

public class GetAllUnarchivedPluginsForProjectIdQueryTest
{
    private GetAllUnarchivedPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;

    [SetUp]
    public void SetUp()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetAllUnarchivedPluginsForProjectIdQueryHandler(_pluginRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_WhenUnarchivedPluginsExist_ReturnsPlugins()
    {
        // Arrange
        var plugins = new List<ProjectPlugins>
        {
            new()
            {
                PluginId = 1,
                Plugin = new Plugin { Id = 1, PluginName = "Plugin 1", IsArchived = false }, // Unarchived
                ProjectId = 1,
                Url = "Plugin1.com",
                DisplayName = "GitLab"
            },
            new()
            {
                PluginId = 2,
                Plugin = new Plugin { Id = 2, PluginName = "Plugin 2", IsArchived = false }, // Unarchived
                ProjectId = 1,
                Url = "Plugin2.com",
                DisplayName = "Jira"
            }
        };

        _pluginRepositoryMock.Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins);

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2)); // Expecting two unarchived plugins
        Assert.That(result[0].Plugin.PluginName, Is.EqualTo("Plugin 1"));
        Assert.That(result[1].Plugin.PluginName, Is.EqualTo("Plugin 2"));
        Assert.That(result[0].Url, Is.EqualTo("Plugin1.com"));
        Assert.That(result[1].Url, Is.EqualTo("Plugin2.com"));
    }

    [Test]
    public async Task Handle_WhenNoUnarchivedPluginsExist_ReturnsEmptyList()
    {
        // Arrange
        var plugins = new List<ProjectPlugins>(); // No plugins found
        _pluginRepositoryMock.Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins);

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(0)); // Expecting an empty list
    }

    [Test]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        _pluginRepositoryMock.Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync((List<ProjectPlugins>)null); // Simulating null return from repository

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Null); // The result should be null
    }

    [Test]
    public async Task Handle_WhenCancellationTokenIsTriggered_AbortsOperation()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // Trigger cancellation immediately

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);

        // Simulate that the repository doesn't matter in this case because the operation will be cancelled
        _pluginRepositoryMock.Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(new List<ProjectPlugins>());

        // Act & Assert
        // The handler should throw an OperationCanceledException when cancellation is triggered
        var ex = Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await _handler.Handle(query, cancellationTokenSource.Token);
        });

        // Assert that the exception is of the expected type
        Assert.That(ex, Is.InstanceOf<OperationCanceledException>());
    }



    [Test]
    public async Task Handle_WhenSomePluginsAreArchived_ReturnsOnlyUnarchivedPlugins()
    {
        // Arrange
        var plugins = new List<ProjectPlugins>
        {
            new()
            {
                PluginId = 1,
                Plugin = new Plugin { Id = 1, PluginName = "Plugin 1", IsArchived = false }, // Unarchived
                ProjectId = 1,
                Url = "Plugin1.com",
                DisplayName = "GitLab"
            },
            new()
            {
                PluginId = 2,
                Plugin = new Plugin { Id = 2, PluginName = "Plugin 2", IsArchived = true }, // Archived
                ProjectId = 1,
                Url = "Plugin2.com",
                DisplayName = "Jira"
            }
        };

        // Ensure the mock only returns unarchived plugins for project ID 1
        _pluginRepositoryMock.Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins.Where(p => !p.Plugin.IsArchived).ToList());

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert that the result contains only 1 unarchived plugin
        Assert.That(result, Has.Count.EqualTo(1)); // Only one unarchived plugin should be returned
        Assert.That(result[0].Plugin.PluginName, Is.EqualTo("Plugin 1")); // Assert the unarchived plugin is "Plugin 1"
    }

}
