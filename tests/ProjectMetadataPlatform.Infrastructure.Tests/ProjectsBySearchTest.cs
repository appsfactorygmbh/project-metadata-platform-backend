using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ProjectsBySearchTest : TestsWithDatabase
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
    public async Task GetProjectsWithSearchTest()
    {
        // Arrange
        var exampleProject = new Project()
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
        var result = await _repository.GetProjectsAsync("ege");
        Assert.IsNotEmpty(result);
        var project = result.First();
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

        var result = await _repository.GetProjectsAsync("x");
        Assert.IsEmpty(result);
    }
    
    [Test] 
    public async Task GetProjectsWithSearch_MultipleMatches_Test()
    {
        // Arrange
        var projects = new List<Project>()
        {
            new Project()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new Project()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "ESA",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new Project()
            {
                Id = 3,
                ProjectName = "Turbo",
                ClientName = "Regen",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            
        };

        foreach (Project proj in projects)
        {
            _context.Projects.Add(proj);
        }
        
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjectsAsync("Reg");

        // Assert
        Assert.AreEqual(2, result.Count());
        
    }
}
