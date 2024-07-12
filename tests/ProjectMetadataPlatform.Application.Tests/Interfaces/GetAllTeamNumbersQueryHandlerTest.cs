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
public class GetAllTeamNumbersQueryHandlerTest
{
    [SetUp]

    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllTeamNumbersQueryHandler(_mockProjectRepo.Object);
    }
    private GetAllTeamNumbersQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetAllTeamNumbersRequestTest()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Rayquaza",
                ClientName = "Silph.co",
                BusinessUnit = "TeamRocket",
                TeamNumber = 42,
                Department = "Indigo Liga"
            },
            new Project(){
                Id = 0,
                ProjectName = "Mars",
                ClientName = "SpaceX",
                BusinessUnit = "Marsianer",
                TeamNumber = 43,
                Department = "Space"

          }
        };
        _mockProjectRepo.Setup(m => m.GetTeamNumbersAsync()).ReturnsAsync(projectsResponseContent.Select(p => p.TeamNumber).Distinct());
        var request = new GetAllTeamNumbersQuery();
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();
        Assert.That(result, Is.InstanceOf<IEnumerable<int>>());
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(42, result[0]);
        Assert.AreEqual(43, result[1]);

    }
    [Test]
    public async Task HandleGetAllTeamNumbersRequest_EmptyList_Test()
    {
        var projectsResponseContent = new List<Project>();
        _mockProjectRepo.Setup(m => m.GetTeamNumbersAsync()).ReturnsAsync(projectsResponseContent.Select(p => p.TeamNumber).Distinct());
        var request = new GetAllTeamNumbersQuery();
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();
        Assert.That(result, Is.InstanceOf<IEnumerable<int>>());
        Assert.AreEqual(0, result.Count);
        Assert.IsEmpty(result);
    }
}
