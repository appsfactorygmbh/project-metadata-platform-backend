using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Interfaces;

[TestFixture]
public class GetAllProjectsQueryHandlerTest
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
    public async Task HandleGetAllProjectsRequest_EmptyResponse_Test()
    {
        _mockProjectRepo.Setup(m => m.GetAllProjectsAsync()).ReturnsAsync([]);

        var result = await _handler.Handle(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>());

        Project[] resultArray = result as Project[] ?? result.ToArray();
        Assert.That(resultArray, Is.Not.Null);
        Assert.That(resultArray, Is.InstanceOf<IEnumerable<Project>>());
        
        Assert.That(resultArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task HandleGetAllProjectsRequest_Test()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };
        _mockProjectRepo.Setup(m => m.GetAllProjectsAsync()).ReturnsAsync(projectsResponseContent);
        
        var result = await _handler.Handle(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>());
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        var resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
        
        
        var project = resultArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(project.Department, Is.EqualTo("Homelandsecurity"));
            Assert.That(project.TeamNumber, Is.EqualTo(42));
        });
    }
}
