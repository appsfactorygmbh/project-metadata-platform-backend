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

public class GetProjectsByBusinessUnitsQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectsByBusinessUnitsQueryHandler(_mockProjectRepo.Object);
    }
    private GetProjectsByBusinessUnitsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task GetProjectsByBusinessUnitsTest_Match()
    {
        var businessUnits = new List<string> { "666", "777" };
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

        _mockProjectRepo.Setup(repo => repo.GetProjectsByBusinessUnitsAsync(businessUnits))
            .ReturnsAsync(projects.Where(p => businessUnits.Contains(p.BusinessUnit)));

        var request = new GetProjectsByBusinessUnitsQuery(businessUnits);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("666", result.First().BusinessUnit);
        Assert.AreEqual("777", result.Last().BusinessUnit);
    }

    [Test]
    public async Task GetProjectsByBusinessUnitsTest_NoMatch()
    {
        var businessUnits = new List<string> { "666", "777" };

        _mockProjectRepo.Setup(repo => repo.GetProjectsByBusinessUnitsAsync(businessUnits))
            .ReturnsAsync(Enumerable.Empty<Project>());

        var request = new GetProjectsByBusinessUnitsQuery(businessUnits);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
}
