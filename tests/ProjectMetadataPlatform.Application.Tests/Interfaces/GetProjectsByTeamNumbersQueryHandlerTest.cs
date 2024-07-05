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

public class GetProjectsByTeamNumbersQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectsByTeamNumbersQueryHandler(_mockProjectRepo.Object);
    }
    private GetProjectsByTeamNumbersQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task GetProjectsByTeamNumbersTest_Match()
    {
        var teamNumbers = new List<int> { 42, 43 };
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

        _mockProjectRepo.Setup(repo => repo.GetProjectsByTeamNumbersAsync(teamNumbers))
            .ReturnsAsync(projects.Where(p => teamNumbers.Contains(p.TeamNumber)));

        var request = new GetProjectsByTeamNumbersQuery(teamNumbers);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Multiple((() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.TeamNumber == 42), Is.True);
            Assert.That(result.Any(p => p.TeamNumber == 43), Is.True);
        }));
    }

    [Test]
    public async Task GetProjectsByTeamNumbersTest_NoMatch()
    {
        var teamNumbers = new List<int> { 42, 43 };

        _mockProjectRepo.Setup(repo => repo.GetProjectsByTeamNumbersAsync(teamNumbers))
            .ReturnsAsync(Enumerable.Empty<Project>());

        var request = new GetProjectsByTeamNumbersQuery(teamNumbers);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
