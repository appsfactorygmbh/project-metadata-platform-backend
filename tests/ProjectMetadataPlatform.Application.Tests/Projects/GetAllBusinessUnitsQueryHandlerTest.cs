using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class GetAllBusinessUnitsQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllBusinessUnitsQueryHandler(_mockProjectRepo.Object);
    }

    private GetAllBusinessUnitsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task GetAllBusinessUnitsTest()
    {
        var businessUnit = new List<string> { "42", "43" };
        _mockProjectRepo.Setup(m => m.GetBusinessUnitsAsync()).ReturnsAsync(businessUnit);

        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.EquivalentTo(businessUnit));
    }
}
