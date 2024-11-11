using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetUserByUserNameQueryHandlerTest
{
    private GetUserByUserNameQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetUserByUserNameQueryHandler(_mockUserRepo.Object);
    }

    [Test]
    public async Task HandleGetUserByUserName_Test()
    {
        var user = new User { Id = "13", Name = "Victor Perkins", UserName = "Vector", Email = "squidlauncher@bankofevil.com" };

        _mockUserRepo.Setup(m => m.GetUserByUserNameAsync("Vector")).ReturnsAsync(user);

        var request = new GetUserByUserNameQuery("Vector");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<User>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("13"));
            Assert.That(result.Name, Is.EqualTo("Victor Perkins"));
            Assert.That(result.UserName, Is.EqualTo("Vector"));
            Assert.That(result.Email, Is.EqualTo("squidlauncher@bankofevil.com"));
        });
    }

    [Test]
    public async Task HandleGetUserByUserName_NotFound_Test()
    {
        _mockUserRepo.Setup(m => m.GetUserByUserNameAsync("Vector")).ReturnsAsync((User)null!);

        var request = new GetUserByUserNameQuery("Bob");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }
}
