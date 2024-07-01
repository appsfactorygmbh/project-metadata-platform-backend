using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class UpdateProjectRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
    }

    [Test]
    public async Task UpdateProjectPluginListTest()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
        };
        var examplePlugin = new Plugin
        {
            Id = 1,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            PluginId = 1,
            ProjectId = 1,
            Url = "dummy",
            DisplayName = "Dummy"
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };
        _context.Plugins.Add(examplePlugin);
        _context.Projects.Add(exampleProject);
        await _context.SaveChangesAsync();
        await _repository.DeletePluginAssociation(exampleProject.Id);
        await _repository.UpdateProject(exampleProject,projectPluginList);
        var projectResult = _context.Projects.FirstOrDefault(p => p.Id == exampleProject.Id);
        Assert.That(projectResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResult.Id, Is.EqualTo(1));
            Assert.That(projectResult.ProjectName, Is.EqualTo("Example Project"));
            Assert.That(projectResult.BusinessUnit, Is.EqualTo("Example Business Unit"));
            Assert.That(projectResult.TeamNumber, Is.EqualTo(1));
            Assert.That(projectResult.Department, Is.EqualTo("Example Department"));
            Assert.That(projectResult.ClientName, Is.EqualTo("Example Client"));
        });
        var pluginResult = _context.ProjectPluginsRelation.Where(p => p.ProjectId == exampleProject.Id).ToList();
        Assert.That(pluginResult, Is.Not.Null);
        Assert.That(pluginResult, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(pluginResult[0].PluginId, Is.EqualTo(1));
            Assert.That(pluginResult[0].ProjectId, Is.EqualTo(1));
            Assert.That(pluginResult[0].Url, Is.EqualTo("dummy"));
            Assert.That(pluginResult[0].DisplayName, Is.EqualTo("Dummy"));
        });
    }
}
