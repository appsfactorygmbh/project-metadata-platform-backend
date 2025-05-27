using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Teams;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class TeamsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new TeamRepository(_context);
        ClearData(_context);
    }

    private ProjectMetadataPlatformDbContext _context;
    private TeamRepository _repository;

    [Test]
    public async Task GetTeamsAsync_ShouldReturnAllTeams()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        var team2 = new Team
        {
            Id = 2,
            TeamName = "Test_2",
            BusinessUnit = "BU Foo",
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        _context.Teams.Add(team2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetTeamsAsync(null, null)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var teamRes = result.First();
        var teamRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(teamRes.Id, Is.EqualTo(1));
            Assert.That(teamRes.TeamName, Is.EqualTo("Test_1"));
            Assert.That(teamRes.PTL, Is.EqualTo("Max Mustermann"));
            Assert.That(teamRes.BusinessUnit, Is.EqualTo("BU Test"));
            Assert.That(teamRes2.Id, Is.EqualTo(2));
            Assert.That(teamRes2.TeamName, Is.EqualTo("Test_2"));
            Assert.That(teamRes2.PTL, Is.EqualTo(null));
            Assert.That(teamRes2.BusinessUnit, Is.EqualTo("BU Foo"));
        });
    }

    [TestCase("", 1, 2, 3)]
    [TestCase("Test", 1, 2)]
    [TestCase("foo bar", 3)]
    [TestCase("abc")]
    public async Task GetTeamByFilterTeamName_ReturnsCorrectProjects(
        string teamName,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "BU Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: teamName, fullTextQuery: null)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [TestCase("", 1, 2, 3)]
    [TestCase("foo bar", 3)]
    [TestCase("abc")]
    [TestCase("Test", 1, 2, 3)]
    [TestCase("bu", 1, 3)]
    [TestCase("muSTermann", 1, 2)]
    public async Task GetTeamByFullTextQuery_ReturnsCorrectProjects(
        string query,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: null, fullTextQuery: query)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [TestCase("Test", "", 1, 2)]
    [TestCase("teSt", "Max", 1)]
    [TestCase("2", "Foo", 2)]
    [TestCase("Foo Bar", "2")]
    [TestCase("", "Bu", 1, 3)]
    public async Task GetTeamByFullTextQueryAndTeamName_ReturnsCorrectProjects(
        string teamName,
        string query,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: teamName, fullTextQuery: query)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var deletedTeam = await _repository.DeleteTeamAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var remainingTeams = _context.Teams.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingTeams, Is.Empty);
            Assert.That(deletedTeam, Is.EqualTo(team));
        });
    }

    [Test]
    public async Task GetTeamWithProjectsAsync_ShouldReturnTeamWithAssociatedProjects()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        var project = new Project
        {
            Id = 1,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            TeamId = 1,
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        // Act
        var teamWithProjects = await _repository.GetTeamWithProjectsAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(teamWithProjects.TeamName, Is.EqualTo("Test_1"));
            Assert.That(teamWithProjects.BusinessUnit, Is.EqualTo("BU Test"));
            Assert.That(teamWithProjects.PTL, Is.EqualTo("Max Mustermann"));
            Assert.That(teamWithProjects.Id, Is.EqualTo(1));

            Assert.That(teamWithProjects.Projects, Is.Not.Null);
            Assert.That(teamWithProjects.Projects, Has.Count.EqualTo(1));
            var linkedProject = teamWithProjects.Projects!.First();

            Assert.That(linkedProject.Id, Is.EqualTo(1));
            Assert.That(linkedProject.ProjectName, Is.EqualTo("Nieselegen"));
            Assert.That(linkedProject.Slug, Is.EqualTo("nieselregen"));
            Assert.That(linkedProject.ClientName, Is.EqualTo("Nasa"));
        });
    }

    [Test]
    public async Task GetTeamAsync_ShouldThrowTeamNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.Teams.RemoveRange(_context.Teams);
        await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNotFoundException>(async () =>
            await _repository.GetTeamAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task RetrieveNameForIdAsync_ShouldReturnCorrectName()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };
        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var teamName = await _repository.RetrieveNameForIdAsync(1);

        // Assert
        Assert.That(teamName, Is.EqualTo("Test_1"));
    }
}
