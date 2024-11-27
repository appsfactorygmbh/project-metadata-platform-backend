using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "camo") }, "TestAuth");
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed

        var httpContext = new DefaultHttpContext
        {
            User = contextUser
        };
        httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);
        httpContextAccessorMock.Setup(_ => _.HttpContext.User).Returns(contextUser);
        _handler = new DeleteUserCommandHandler(_mockUsersRepo.Object, httpContextAccessorMock.Object);
    }
    private DeleteUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IHttpContextAccessor> httpContextAccessorMock;

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

    [Test]
    public async Task DeleteUser_SelfDeletionAttempt_Test()
    {
        var user = new User {Email = "camo", Id = "1"};
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.GetUserByEmailAsync("camo")).ReturnsAsync(user);
        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None));
    }
}
