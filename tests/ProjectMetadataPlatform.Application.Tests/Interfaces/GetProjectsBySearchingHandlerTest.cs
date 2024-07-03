using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

[TestFixture]
public class GetProjectsBySearchingHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(_mockProjectRepo.Object);
    }
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectBySearchRequest_NonexistentProject_Test()
    {
        Project[] emptyProjectList = Array.Empty<Project>();

        _mockProjectRepo.Setup(m => m.GetProjectsAsync("M")).ReturnsAsync(emptyProjectList);
        var query = new GetAllProjectsQuery("M");
        IEnumerable<Project> result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task HandleGetProjectRequestBySearching_Test()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync("R")).ReturnsAsync(projectsResponseContent);
        var query = new GetAllProjectsQuery("R");
        IEnumerable<Project> result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Project>>());
        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }
    [Test]
    public async Task HandleGetProjectRequestBySearchingWithNullSearch_Test()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            new()
            {
                Id = 3,
                ProjectName = "Sonne",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };
        _mockProjectRepo.Setup(m => m.GetProjectsAsync()).ReturnsAsync(projectsResponseContent);
        var query = new GetAllProjectsQuery("");
        IEnumerable<Project> result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Project>>());
        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }
}
