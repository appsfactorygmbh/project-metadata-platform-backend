using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Domain.Plugins;

[TestFixture]
public class ProjectMetadataPlatformDbContextTests
{
    protected ProjectMetadataPlatformDbContext _context;

    [SetUp]
    public void Setup()
    {
        _context = new(
            new DbContextOptionsBuilder<ProjectMetadataPlatformDbContext>()
                .UseSqlite("Datasource=test-db.db").Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        // Seed the database with initial data
        InitDatabase();
    }

    private void InitDatabase()
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
    }

    [Test]
    public async Task TryRetrievingProject()
    {
        // Act
        var projects = await _context.Projects.ToListAsync();

        // Assert
        Assert.AreEqual(1, projects.Count);
        Assert.AreEqual(1, projects.First().Id);
        Assert.AreEqual("Regen", projects.First().ProjectName);
        Assert.AreEqual("Nasa", projects.First().ClientName);
        Assert.AreEqual("BuWeather", projects.First().BusinessUnit);
        Assert.AreEqual(42, projects.First().TeamNumber);
        Assert.AreEqual("Homelandsecurity", projects.First().Department);
    }

    [Test]
    public async Task TryAddingNewProject()
    {
        // Arrange
        var newProject = new Project()
        {
            Id = 2,
            ProjectName = "Sonnenschein",
            ClientName = "Weltraum",
            BusinessUnit = "Galaxie",
            TeamNumber = 13,
            Department = "Atemlos"
        };

        // Act
        _context.Projects.Add(newProject);
        await _context.SaveChangesAsync();

        var projects = await _context.Projects.ToListAsync();

        // Assert
        Assert.AreEqual(2, projects.Count);
        var addedProject = projects.FirstOrDefault(p => p.Id == 2);
        Assert.IsNotNull(addedProject);
        Assert.AreEqual("Sonnenschein", addedProject.ProjectName);
        Assert.AreEqual("Weltraum", addedProject.ClientName);
        Assert.AreEqual("Galaxie", addedProject.BusinessUnit);
        Assert.AreEqual(13, addedProject.TeamNumber);
        Assert.AreEqual("Atemlos", addedProject.Department);
    }
    [Test]
    public async Task GettingPluginsForProject()
    {
        // Arrange
        var rela = await _context.ProjectPluginsRelation.Include(projectPlugins => projectPlugins.Plugin).ToListAsync();
        // Act
        List<GetPluginResponse> reponses = new List<GetPluginResponse>();
        for (int i = 0; i < rela.Count; i++)
        {
            var obj = rela[i];
            GetPluginResponse plugin = new(obj.Plugin?.PluginName, obj.Url, obj.DisplayName);
            reponses.Add(plugin);
        }
        
        Assert.That(reponses, Is.Not.Empty);
        GetPluginResponse pluginRes = reponses[0];
        
        Assert.Multiple(() =>
        {
            Assert.That(pluginRes.Url, Is.EqualTo("gitlab.com"));
            Assert.That(pluginRes.DisplayName, Is.EqualTo("gitlab"));
            Assert.That(pluginRes.PluginName, Is.EqualTo("Gitlab"));
        });
    }
}
