using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetAllProjectsQueryHandlerTest
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
    public async Task HandleGetAllProjectsRequest_EmptyResponse_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectsAsync()).ReturnsAsync([]);
        var request = new GetAllProjectsQuery(null, "");
        IEnumerable<Project> result = await _handler.Handle(request, It.IsAny<CancellationToken>());

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
                Id = 0,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projectsResponseContent);
        var request = new GetAllProjectsQuery(null, "");
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));


        Project project = resultArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(0));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
            Assert.That(project.Department, Is.EqualTo("Homelandsecurity"));
            Assert.That(project.TeamNumber, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task HandleGetProjectsByFiltersRequest_Test()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Metatron",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            null,
            new List<string>{"Ag der Ags"},
            SecurityLevel.HIGH
        );
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH
            },
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projects.Where(
            p => filters.ProjectName.Contains(p.ProjectName) &&
                 filters.ClientName.Contains(p.ClientName) &&
                 filters.BusinessUnit.Contains(p.BusinessUnit) &&
                 filters.TeamNumber.Contains(p.TeamNumber) &&
                 filters.Company.Contains(p.Company) &&
                 filters.IsmsLevel == p.IsmsLevel));
        var request = new GetAllProjectsQuery(filters, "");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
    }

    [Test]
    public async Task HandleGetProjectsByFiltersRequest_NoMatch_Test()
    {
        var filters = new ProjectFilterRequest
        (
            "Heather",
            "Gilgamesh",
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            true,
            new List<string> {"Unknown"},
            SecurityLevel.HIGH
        );
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                Company = "High Five",
                IsmsLevel = SecurityLevel.HIGH

            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                Company = "Space Y",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Unknown",
                IsmsLevel = SecurityLevel.HIGH
            },
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projects.Where(
            p => filters.ProjectName.Contains(p.ProjectName) &&
                 filters.ClientName.Contains(p.ClientName) &&
                 filters.BusinessUnit.Contains(p.BusinessUnit) &&
                 filters.TeamNumber.Contains(p.TeamNumber) &&
                 filters.Company.Contains(p.Company) &&
                 filters.IsmsLevel == p.IsmsLevel));
        var request = new GetAllProjectsQuery(filters, "");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task HandleGetProjectsByFiltersRequestAndSearch_Match_Test()
    {
        var search = "Hea";
        var filters = new ProjectFilterRequest
        (
            null,
            null,
            new List<string> { "666", "777" },
            new List<int> { 42, 43 },
            false,
            null,
            null
        );
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                IsArchived = false
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                IsArchived = false
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                IsArchived = true
            },
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projects.Where(
            p => p.ProjectName.ToLower().Contains(search.ToLower()) &&
                 filters.BusinessUnit.Contains(p.BusinessUnit) &&
                 filters.TeamNumber.Contains(p.TeamNumber)));
        var request = new GetAllProjectsQuery(filters, search);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
    }

    [Test]
    public async Task HandleGetProjects_NoFilterAndSearch_Test()
    {
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                Company = "Unknown",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Lucifer",
                Department = "Venus",
                TeamNumber = 43,
                Company = "Unknown",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Unknown",
                IsmsLevel = SecurityLevel.HIGH
            },
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projects);
        var request = new GetAllProjectsQuery(null, "");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(3));
    }
     [Test]
    public async Task HandleGetProjectsAlphabetical_Test()
    {
        var projects = new List<Project>
        {
            new Project
            {
                Id = 5,
                ProjectName = "Aapfel",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Zatan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 1,
                ProjectName = "Beta",
                Slug = "heather",
                BusinessUnit = "666",
                ClientName = "Metatron",
                Department = "Mars",
                TeamNumber = 42,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 2,
                ProjectName = "Apfel",
                Slug = "james",
                BusinessUnit = "777",
                ClientName = "Metatron",
                Department = "Venus",
                TeamNumber = 43,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH
            },
            new Project
            {
                Id = 4,
                ProjectName = "Aarika",
                Slug = "marika",
                BusinessUnit = "999",
                ClientName = "Satan",
                Department = "Earth",
                TeamNumber = 44,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH
            },
        };

        _mockProjectRepo.Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>())).ReturnsAsync(projects);
        var request = new GetAllProjectsQuery(null, null);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<Project>>());

        Project[] resultArray = result.ToArray();
        Assert.That(resultArray[0].Id, Is.EqualTo(2) );
        Assert.That(resultArray[1].Id, Is.EqualTo(1) );
        Assert.That(resultArray[2].Id, Is.EqualTo(4) );
        Assert.That(resultArray[3].Id, Is.EqualTo(3) );
        Assert.That(resultArray[4].Id, Is.EqualTo(5) );
    }
}
