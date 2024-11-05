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
public class GetUserQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetUserQueryHandler(_mockUserRepo.Object);
    }
    private GetUserQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [Test]
    public async Task HandleGetUserRequest_Test()
    {
        var userResponseContent = new User
        {
            Id = "1",
            Name = "Hinz"
        };

        _mockUserRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(userResponseContent);
        var request = new GetUserQuery("1");
        User? result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<User>());
        Assert.Multiple((() =>
        {
            Assert.That(result.Id, Is.EqualTo("1"));
            Assert.That(result.Name, Is.EqualTo("Hinz"));
        }));
    }

    [Test]
    public async Task HandleGetUserRequest_NonexistentUser_Test()
    {
        _mockUserRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync((User?)null);
        var request = new GetUserQuery("1");
        User? result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }
}
