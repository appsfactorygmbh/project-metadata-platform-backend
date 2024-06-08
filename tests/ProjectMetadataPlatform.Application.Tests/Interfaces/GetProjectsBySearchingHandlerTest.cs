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
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(_mockProjectRepo.Object);
    }

    [Test]
    public async Task HandleGetProjectBySearchRequest_NonexistentProject_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectsAsync("M")).ReturnsAsync((List<Project>)null);
        var query= new GetAllProjectsQuery("M");
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task HandleGetProjectRequestBySearching_Test()
    {
        var projectsResponseContent = new List<Project>()
        {
            new Project
            {
                Id = 2,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            },
            
            
        };
         
        _mockProjectRepo.Setup(m => m.GetProjectsAsync("R")).ReturnsAsync(projectsResponseContent);
        var query= new GetAllProjectsQuery("R");
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Project>>());
        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }
    
}
