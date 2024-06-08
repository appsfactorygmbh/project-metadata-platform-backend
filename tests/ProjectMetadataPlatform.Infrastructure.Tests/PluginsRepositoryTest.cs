using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Plugins.Models;
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
        var project = new Project()
        {
            Id = 1,
            ProjectName = "Regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.Add(project);

        var plugin = new Plugin() { Id = 1, PluginName = "Gitlab", };
        _context.Plugins.Add(plugin);

        var projectPluginRelation = new ProjectPlugins()
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

        var rep = await _repository.GetAllPluginsForProjectIdAsync(1);
        
        Assert.That(rep, Is.Not.Empty);
        
        Assert.Multiple(() =>
        {
            Assert.That(rep[0].Url, Is.EqualTo("gitlab.com"));
            Assert.That(rep[0].DisplayName, Is.EqualTo("gitlab"));
            Assert.That(rep[0].Plugin.PluginName, Is.EqualTo("Gitlab"));
        });
    }
    
}
