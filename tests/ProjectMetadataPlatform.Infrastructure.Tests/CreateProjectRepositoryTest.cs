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
        var project = await _repository.UpdateWithReturnValue(exampleProject);
        Assert.That(project, Is.Not.Null);
        Assert.That(project.ProjectName, Is.EqualTo("Example Project"));
        Assert.That(project.BusinessUnit, Is.EqualTo("Example Business Unit"));
        Assert.That(project.TeamNumber, Is.EqualTo(1));
        Assert.That(project.ClientName, Is.EqualTo("Example Client"));
        Assert.That(project.Department, Is.EqualTo("Example Department"));
        Assert.That(project.Id, Is.GreaterThan(0));
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
        var project = await _repository.UpdateWithReturnValue(exampleProject);
        Assert.That(project, Is.Not.Null);
        Assert.That(project.ProjectName, Is.EqualTo("Example Project"));
        Assert.That(project.BusinessUnit, Is.EqualTo("Example Business Unit"));
        Assert.That(project.TeamNumber, Is.EqualTo(1));
        Assert.That(project.ClientName, Is.EqualTo("Example Client"));
        Assert.That(project.Department, Is.EqualTo("Example Department"));
        Assert.That(project.Id, Is.EqualTo(1));
    }
}
