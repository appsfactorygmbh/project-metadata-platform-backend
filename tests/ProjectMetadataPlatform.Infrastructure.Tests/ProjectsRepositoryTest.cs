using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ProjectsRepositoryTests : TestsWithDatabase
{

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
        ClearData(_context);
    }
    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [Test]
    public async Task GetAllProjectsAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        IEnumerable<Project> result = await _repository.GetProjectsAsync();

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.That(project.Id, Is.EqualTo(1));
        Assert.That(project.ProjectName, Is.EqualTo("Regen"));
        Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(project.TeamNumber, Is.EqualTo(42));
        Assert.That(project.Department, Is.EqualTo("Homelandsecurity"));
    }
}
