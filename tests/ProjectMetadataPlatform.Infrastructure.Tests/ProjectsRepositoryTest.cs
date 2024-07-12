using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

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

    [Test]
    public async Task GetProjectByMultipleFiltersAndSearchAsync_ReturnsCorrectProjects()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Metatron",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 }
        );
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44
            },
        };

        var query = new GetAllProjectsQuery(filters, "Hea");

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.Multiple((() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.ProjectName == "Heather"), Is.True);
            Assert.That(result.Any(p => p.ClientName == "Metatron"), Is.True);
            Assert.That(result.Any(p => p.BusinessUnit == "666"), Is.True);
            Assert.That(result.Any(p => p.TeamNumber == 42), Is.True);
        }));
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoMatchingProjects_ReturnsEmpty()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Gilgamesch",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 }
        );
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43
            },
        };

        var query = new GetAllProjectsQuery(filters, null);

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoFilters_ReturnsAllProjects()
    {
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44
            },
        };

        var query = new GetAllProjectsQuery(null, null);

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.Multiple((() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }));
    }
    [Test]
    public async Task GetAllTeamNumbersAsync_ReturnAllTeamNumbers()
    {
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new Project
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 43,
                Department = "Homelandsecurity"
            }
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        IEnumerable<int> result = await _repository.GetTeamNumbersAsync();

        Assert.AreEqual(2, result.Count());
        Assert.That(result, Is.EquivalentTo(new[] { 42, 43 }));
    }
}
