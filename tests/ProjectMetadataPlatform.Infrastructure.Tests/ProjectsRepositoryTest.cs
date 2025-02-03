using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

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
            Slug = "regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetProjectsAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        project = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(project.TeamNumber, Is.EqualTo(42));
            Assert.That(project.Department, Is.EqualTo("Homelandsecurity"));
        });
    }

    [Test]
    public async Task GetProjectByMultipleFiltersAndSearchAsync_ReturnsCorrectProjects()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Metatron",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            true,
            new List<string> { "AppsFact" },
            SecurityLevel.VERY_HIGH
        );
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                IsArchived = true,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                IsArchived = true,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                IsArchived = false,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH
            },
        };

        var query = new GetAllProjectsQuery(filters, "Hea");

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = (await _repository.GetProjectsAsync(query)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(p => p.ProjectName == "Heather"), Is.True);
            Assert.That(result.Any(p => p.ClientName == "Metatron"), Is.True);
            Assert.That(result.Any(p => p.BusinessUnit == "666"), Is.True);
            Assert.That(result.Any(p => p.TeamNumber == 42), Is.True);
            Assert.That(result.Any(p => p.IsArchived), Is.True);
            Assert.That(result.Any(p => p.Company == "AppsFact"), Is.True);
            Assert.That(result.Any(p => p.IsmsLevel == SecurityLevel.VERY_HIGH), Is.True);
        });
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoMatchingProjects_ReturnsEmpty()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Gilgamesch",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            null,
            new List<string> { "Nothing else" },
            null
        );
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                Company = "AddOn",
                IsmsLevel = SecurityLevel.HIGH
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                Company = "AddOn",
                IsmsLevel = SecurityLevel.HIGH
            },
        };

        var query = new GetAllProjectsQuery(filters, null);

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoFilters_ReturnsAllProjects()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
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

        Assert.That(result.Count(), Is.EqualTo(3));
    }
    [Test]
    public async Task GetAllTeamNumbersAsync_ReturnAllTeamNumbers()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 2,
                ProjectName = "Nieselegen",
                Slug = "nieselregen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 43,
                Department = "Homelandsecurity"
            }
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetTeamNumbersAsync();

        var expectedTeamNumbers = new[] { 42, 43 };
        Assert.That(result, Is.EquivalentTo(expectedTeamNumbers));
    }

    [Test]
    public async Task GetAllBusinessUnitsAsync_ReturnsAllBusinessUnits()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Project1",
                Slug = "project1",
                ClientName = "ClientA",
                BusinessUnit = "Unit1",
                TeamNumber = 42,
                Department = "Dept1"
            },
            new()
            {
                Id = 2,
                ProjectName = "Project2",
                Slug = "project2",
                ClientName = "ClientB",
                BusinessUnit = "Unit2",
                TeamNumber = 43,
                Department = "Dept2"
            }
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetBusinessUnitsAsync();

        var expectedBusinessUnits = new[] { "Unit1", "Unit2" };
        Assert.That(result, Is.EquivalentTo(expectedBusinessUnits));
    }

    [Test]
    public async Task GetAllBusinessUnitsAsync_WithDuplicateBusinessUnits_ReturnsDistinctBusinessUnits()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Project1",
                Slug = "project1",
                ClientName = "ClientA",
                BusinessUnit = "Unit1",
                TeamNumber = 42,
                Department = "Dept1"
            },
            new()
            {
                Id = 2,
                ProjectName = "Project2",
                Slug = "project2",
                ClientName = "ClientB",
                BusinessUnit = "Unit1", // Duplicate BusinessUnit
                TeamNumber = 43,
                Department = "Dept2"
            },
            new()
            {
                Id = 3,
                ProjectName = "Project3",
                Slug = "project3",
                ClientName = "ClientC",
                BusinessUnit = "Unit2",
                TeamNumber = 44,
                Department = "Dept3"
            }
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetBusinessUnitsAsync();

        var expectedBusinessUnits = new[] { "Unit1", "Unit2" };
        Assert.That(result, Is.EquivalentTo(expectedBusinessUnits));
    }

    [Test]
    public async Task GetAllBusinessUnitsAsync_WhenNoProjects_ReturnsEmpty()
    {
        _context.Projects.RemoveRange(_context.Projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetBusinessUnitsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var deletedProject = await _repository.DeleteProjectAsync(project);
        await _context.SaveChangesAsync();
        var remainingProjects = await _context.Projects.ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(remainingProjects, Is.Empty);
            Assert.That(deletedProject, Is.EqualTo(project));
        });
    }

    [Test]
    public async Task GetProjectBySlug_Test()
    {
        var project1 = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectIdBySlugAsync("regen");

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GetProjectBySlug_Test_NotFoundReturnsNull()
    {
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            BusinessUnit = "BuWeather",
            TeamNumber = 42,
            Department = "Homelandsecurity"
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        Assert.ThrowsAsync<ProjectNotFoundException>(() => _repository.GetProjectIdBySlugAsync("regen"));
    }
}