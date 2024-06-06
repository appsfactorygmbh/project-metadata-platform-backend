using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using System.Linq;
using System.Threading.Tasks;

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
}
