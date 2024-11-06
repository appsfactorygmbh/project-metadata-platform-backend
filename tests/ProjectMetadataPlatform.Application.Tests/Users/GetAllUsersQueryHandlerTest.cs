using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetAllUsersQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetAllUsersQueryHandler(_mockUserRepo.Object);
    }
    private GetAllUsersQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [Test]
    public async Task HandleGetAllUsersRequest_EmptyResponse_Test()
    {
        _mockUserRepo.Setup(m => m.GetAllUsersAsync()).ReturnsAsync([]);
        var request = new GetAllUsersQuery();
        IEnumerable<User> result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        User[] resultArray = result as User[] ?? result.ToArray();
        Assert.That(resultArray, Is.Not.Null);
        Assert.That(resultArray, Is.InstanceOf<IEnumerable<User>>());

        Assert.That(resultArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task HandleGetAllUsersRequest_Test()
    {
        var usersResponseContent = new List<User>
        {
            new()
            {
                Id = "1",
                Name = "Hinz"
            }
        };

        _mockUserRepo.Setup(m => m.GetAllUsersAsync()).ReturnsAsync(usersResponseContent);
        var request = new GetAllUsersQuery();
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<User>>());

        User[] resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
        Assert.Multiple((() =>
        {
            Assert.That(resultArray[0].Id, Is.EqualTo("1"));
            Assert.That(resultArray[0].Name, Is.EqualTo("Hinz"));
        }));
    }
}
