using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class DeleteUserCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _handler = new DeleteUserCommandHandler(_mockUsersRepo.Object);
    }
    private DeleteUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new User { Id = "1" };
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.DeleteUserAsync(user)).ReturnsAsync(user);

        var result = await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);

        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task DeleteUser_InvalidUser_Test()
    {
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync((User)null);

        var result = await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);

        Assert.That(result, Is.EqualTo(null));
    }
}
