using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class GetAllTeamNumberQueryHandlerTest
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
    public async Task GetAllTeamNumbersTest()
    {
        IEnumerable<int> projectsResponseContent = new List<int>()
        {
            42,
            43
        };
        _mockProjectRepo.Setup(m => m.GetTeamNumbersAsync()).ReturnsAsync(projectsResponseContent);

        var query = new GetAllTeamNumbersQuery();
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EquivalentTo(projectsResponseContent));
    }
}
