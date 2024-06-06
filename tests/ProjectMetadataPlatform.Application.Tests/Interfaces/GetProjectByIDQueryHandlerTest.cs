
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;

using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

[TestFixture]
public class ProjectByIDQueryHandlerTest
{
    private GetProjectQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectQueryHandler(_mockProjectRepo.Object);
    }

    [Test]
    public async Task HandleGetAllProjectsRequest_NonexistentProject_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectAsync(2)).ReturnsAsync((Project)null);
        var query= new GetProjectQuery(2);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.That(result, Is.Null);
    }
       
    

    [Test]
    public async Task HandleGetAllProjectsRequest_Test()
    {
        var projectsResponseContent = new Project
        {
            
                Id=2,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(2)).ReturnsAsync(projectsResponseContent);
        var query= new GetProjectQuery(2);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<Project>());

        
        
        
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ProjectName, Is.EqualTo("Regen"));
            Assert.That(result.ClientName, Is.EqualTo("Nasa"));
            Assert.That(result.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(result.Department, Is.EqualTo("Homelandsecurity"));
            Assert.That(result.TeamNumber, Is.EqualTo(42));
            Assert.That(result.Id, Is.EqualTo(2));
        });
    }
    
 
}
