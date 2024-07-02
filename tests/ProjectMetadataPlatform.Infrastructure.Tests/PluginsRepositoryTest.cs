using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class PluginsRepositoryTest : TestsWithDatabase
{

    protected ProjectMetadataPlatformDbContext _context;
    private PluginRepository _repository;
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new PluginRepository(_context);
        DeleteContext(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task TestPluginRepository()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.Add(project);

        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _context.Plugins.Add(plugin);

        var projectPluginRelation = new ProjectPlugins
        {
            PluginId = 1,
            ProjectId = 1,
            Plugin = plugin,
            Project = project,
            Url = "gitlab.com",
            DisplayName = "gitlab"
        };
        _context.Add(projectPluginRelation);
        _context.SaveChanges();

        List<ProjectPlugins> rep = await _repository.GetAllPluginsForProjectIdAsync(1);

        Assert.That(rep, Is.Not.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(rep[0].Url, Is.EqualTo("gitlab.com"));
            Assert.That(rep[0].DisplayName, Is.EqualTo("gitlab"));
            Assert.That(rep[0].Plugin.PluginName, Is.EqualTo("Gitlab"));
        });
    }

    [Test]
    public async Task CreatePlugin_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Warp-Drive", ProjectPlugins = [] };

        Plugin plugin = await _repository.StorePlugin(examplePlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.That(plugin.PluginName, Is.EqualTo("Warp-Drive"));
    }

    [Test]
    public async Task CreatePlugins_IdsDifferent_Test()
    {
        var pluginMethane = new Plugin { PluginName = "Methane", ProjectPlugins = [] };
        var pluginOxygen = new Plugin { PluginName = "Oxygen", ProjectPlugins = [] };

        Plugin pluginOne = await _repository.StorePlugin(pluginMethane);
        Plugin pluginTwo = await _repository.StorePlugin(pluginOxygen);

        Assert.Multiple(() =>
        {
            Assert.That(pluginOne, Is.Not.Null);
            Assert.That(pluginTwo, Is.Not.Null);
        });

        Assert.That(pluginOne.Id, Is.Not.EqualTo(pluginTwo.Id));
    }

    [Test]
    public async Task StorePlugin_noIdIncrementWhenIdExists_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Warp-Drive", ProjectPlugins = [], Id = 42 };
        _context.Add(examplePlugin);
        _context.SaveChanges();

        examplePlugin.PluginName = "Hall Effect Thruster";

        Plugin plugin = await _repository.StorePlugin(examplePlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(plugin.PluginName, Is.EqualTo("Hall Effect Thruster"));
            Assert.That(plugin.Id, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task GetGlobalPluginById_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Warp-Drive", ProjectPlugins = [], Id = 42 };
        _context.Add(examplePlugin);
        _context.SaveChanges();

        var plugin = await _repository.GetPluginByIdAsync(42);

        Assert.That(plugin, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.PluginName, Is.EqualTo("Warp-Drive"));
            Assert.That(plugin.Id, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task GetGlobalPluginById_NotFound_Test()
    {
        var plugin = await _repository.GetPluginByIdAsync(42);

        Assert.That(plugin, Is.Null);
    }
}
