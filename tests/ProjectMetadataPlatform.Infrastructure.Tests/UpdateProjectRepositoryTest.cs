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
            Plugin = examplePlugin,
            Project = exampleProject,
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
        var result = _context.Projects.Where(p => p.Id == exampleProject.Id);
        Assert.NotNull(result);

    }
}
