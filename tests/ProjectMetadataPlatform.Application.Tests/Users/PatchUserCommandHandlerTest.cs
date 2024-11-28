using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class PatchUserCommandHandlerTest
{
    private PatchUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IPasswordHasher<User>> _mockPasswordHasher;
    private Mock<IUnitOfWork> _unitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new PatchUserCommandHandler(_mockUsersRepo.Object, _mockPasswordHasher.Object, _unitOfWork.Object);
    }

    [Test]
    public async Task PatchUser_Test()
    {
        var user = new User { Id = "42",  Email = "candela@hip-hop.dancehall" };
        var newUser = new User { Id = "42", Email = "angela@hip-hop.dancehall" };

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo.Setup(repo => repo.StoreUser(It.IsAny<User>())).ReturnsAsync((User p) => p);

        var result =
            await _handler.Handle(new PatchUserCommand("42","angela@hip-hop.dancehall"), It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(newUser.Email));
            Assert.That(result.Id, Is.EqualTo(newUser.Id));
        });
    }

    [Test]
    public async Task PatchUser_ChangeNothing_Test()
    {
        var user = new User { Id = "42", Email = "cold@play.co.uk" };

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo.Setup(repo => repo.StoreUser(It.IsAny<User>())).ReturnsAsync((User p) => p);

        var result =
            await _handler.Handle(new PatchUserCommand("42"), It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo(user.Email));
            Assert.That(result.Id, Is.EqualTo(user.Id));
        });
    }

    [Test]
    public async Task PatchUser_NotFound_Test()
    {
        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync((User)null!);

        var result =
            await _handler.Handle(new PatchUserCommand("42"), It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }

}
