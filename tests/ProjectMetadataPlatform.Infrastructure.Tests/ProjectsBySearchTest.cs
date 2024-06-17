using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ProjectsBySearchTest : TestsWithDatabase
{


    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
        DeleteContext(_context);
    }
    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [Test]
    public async Task GetProjectsWithSearchTest()
    {
        // Arrange
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.Add(exampleProject);
        await _context.SaveChangesAsync();

        // Act
        IEnumerable<Project> result = await _repository.GetProjectsAsync("ege");
        Assert.IsNotEmpty(result);
        Project project = result.First();
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
    public async Task GetProjectsWithSearch_WithoutMatch_Test()
    {
        var project = new Project
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

        IEnumerable<Project> result = await _repository.GetProjectsAsync("x");
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetProjectsWithSearch_MultipleMatches_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "ESA",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                ClientName = "Regen",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        IEnumerable<Project> result = await _repository.GetProjectsAsync("Reg");

        // Assert
        Assert.AreEqual(2, result.Count());

    }

    [Test]
    public async Task GetProjectsWithSearch_Ignorecase_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "ESA",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                ClientName = "Regen",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        IEnumerable<Project> result = await _repository.GetProjectsAsync("Reg");

        // Assert
        Assert.AreEqual(2, result.Count());

        IEnumerable<Project> resultIgnoreCase = await _repository.GetProjectsAsync("EGen");

        // Assert
        Assert.AreEqual(2, resultIgnoreCase.Count());
    }

    [Test]
    public async Task GetProjectsWithTeamNumber_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "ESA",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                ClientName = "Regen",
                BusinessUnit = "BuWeather",
                TeamNumber = 41,
                Department = "Homelandsecurity"
            }
        };
        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        IEnumerable<Project> resultExactDouble = await _repository.GetProjectsAsync("42");

        // Assert
        Assert.AreEqual(2, resultExactDouble.Count());

        IEnumerable<Project> resultExactSingle = await _repository.GetProjectsAsync("41");
        // Assert
        Assert.AreEqual(1, resultExactSingle.Count());

        IEnumerable<Project> resultOnlyFirstNumber = await _repository.GetProjectsAsync("4");
        // Assert
        Assert.AreEqual(3, resultOnlyFirstNumber.Count());

        IEnumerable<Project> resultOnlySecondNumberNumber = await _repository.GetProjectsAsync("2");
        // Assert
        Assert.AreEqual(2, resultOnlySecondNumberNumber.Count());
    }
}
