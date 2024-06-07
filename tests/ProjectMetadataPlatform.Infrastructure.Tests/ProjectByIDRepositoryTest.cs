using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Domain.Projects;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class ProjectByIDRepositoryTest : TestsWithDatabase
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
    public async Task GetProjectByIDAsync_NonexistentProject()
    {
        // Act
        var result = await _repository.GetProjectAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task GetProjectByIDAsync_ReturnProject()
    {
        // Arrange
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
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjectAsync(1);

        // Assert
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.ProjectName, Is.EqualTo("Regen"));
        Assert.That(result.ClientName, Is.EqualTo("Nasa"));
        Assert.That(result.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(result.TeamNumber, Is.EqualTo(42));
        Assert.That(result.Department, Is.EqualTo("Homelandsecurity"));
        
    }
    
}
