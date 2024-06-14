using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Tests;
[TestFixture]
public class CreateProjectRepositoryTest : TestsWithDatabase
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
    public async Task CreateProject_Test()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client"
        };
        await _repository.AddOrUpdate(exampleProject);
        Assert.That(exampleProject, Is.Not.Null);
        Assert.That(exampleProject.ProjectName, Is.EqualTo("Example Project"));
        Assert.That(exampleProject.BusinessUnit, Is.EqualTo("Example Business Unit"));
        Assert.That(exampleProject.TeamNumber, Is.EqualTo(1));
        Assert.That(exampleProject.ClientName, Is.EqualTo("Example Client"));
        Assert.That(exampleProject.Department, Is.EqualTo("Example Department"));
        Assert.That(exampleProject.Id, Is.GreaterThan(0));
    }
    
    [Test]
    public async Task CreateProject_ProjectAlreadyExists_Test()
    {
        
        var exampleProject = new Project
        {
            Id=1,
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client"
        };
        await _repository.AddOrUpdate(exampleProject);
        var firstresult = await _repository.GetProjectsAsync();
        await _repository.AddOrUpdate(exampleProject);
        Assert.That(exampleProject, Is.Not.Null);
        Assert.That(exampleProject.ProjectName, Is.EqualTo("Example Project"));
        Assert.That(exampleProject.BusinessUnit, Is.EqualTo("Example Business Unit"));
        Assert.That(exampleProject.TeamNumber, Is.EqualTo(1));
        Assert.That(exampleProject.ClientName, Is.EqualTo("Example Client"));
        Assert.That(exampleProject.Department, Is.EqualTo("Example Department"));
        Assert.That(exampleProject.Id, Is.GreaterThan(0));
        var result = await _repository.GetProjectsAsync();
        Assert.AreEqual(result.Count(), firstresult.Count());

        
    }
}
