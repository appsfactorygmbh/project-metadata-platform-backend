using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Logs;
using UserAction = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class DeleteUserCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "camo")], "TestAuth");
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = contextUser
        };
        httpContextAccessorMock.Setup(contextAccessor => contextAccessor.HttpContext).Returns(httpContext);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepository = new Mock<ILogRepository>();
        _mockLogRepository.Setup(repository => repository.GetLogsWithSearch(It.IsAny<string>())).ReturnsAsync([]);
        _handler = new DeleteUserCommandHandler(_mockUsersRepo.Object, httpContextAccessorMock.Object,_mockLogRepository.Object,_mockUnitOfWork.Object);
        httpContextAccessorMock.Setup(contextAccessor => contextAccessor.HttpContext.User).Returns(contextUser);
    }

    private DeleteUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepository;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new IdentityUser { Id = "1", Email = "user@example.com"};
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.DeleteUserAsync(user)).ReturnsAsync(user);
        var result = await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);
        _mockLogRepository.Verify(
            m => m.AddUserLogForCurrentUser(
                It.Is<IdentityUser>(u => u.Id == "1"),
                UserAction.REMOVED_USER,
                It.Is<List<LogChange>>(changes => changes.Count == 1 &&
                                                  changes[0].OldValue == "user@example.com" &&
                                                  changes[0].NewValue == "" &&
                                                  changes[0].Property == nameof(IdentityUser.Email))
            ),
            Times.Once
        );
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    [Description("""
                 Scenario: A user is being deleted. The email of the user has been changed in the past, from 'old@mail.de' to 'user@example.com'.
                 Some logs have been created by the user to be deleted, or affecting the user to be deleted. They were created before the email change.

                 Expected: The email of the logs should be adjusted to the latest email of the user to be deleted.
                 """)]
    public async Task AdjustsEmailOfLogsCreatedByOrAffectingUserToDelete()
    {
        var user = new IdentityUser { Id = "1", Email = "user@example.com"};
        _mockUsersRepo.Setup(repository => repository.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(repository => repository.DeleteUserAsync(user)).ReturnsAsync(user);

        var logByUserToDelete = new Log
        {
            AuthorId = "1", AuthorEmail = "old@mail.de", AffectedUserId = "2", AffectedUserEmail = "different@mail.de"
        };
        var logAffectingUserToDelete = new Log
        {
            AuthorId = "2", AuthorEmail = "different@mail.de", AffectedUserId = "1", AffectedUserEmail = "old@mail.de"
        };
        var logThatShouldNotBeChanged = new Log
        {
            AuthorId = "2", AuthorEmail = "different@mail.de", AffectedUserId = "2", AffectedUserEmail = "different@mail.de"
        };
        _mockLogRepository.Setup(repository => repository.GetLogsWithSearch(It.IsAny<string>()))
            .ReturnsAsync([logByUserToDelete, logAffectingUserToDelete, logThatShouldNotBeChanged]);

        await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(logByUserToDelete.AuthorEmail, Is.EqualTo("user@example.com"));
            Assert.That(logByUserToDelete.AffectedUserEmail, Is.EqualTo("different@mail.de"));
            Assert.That(logAffectingUserToDelete.AuthorEmail, Is.EqualTo("different@mail.de"));
            Assert.That(logAffectingUserToDelete.AffectedUserEmail, Is.EqualTo("user@example.com"));
            Assert.That(logThatShouldNotBeChanged.AuthorEmail, Is.EqualTo("different@mail.de"));
            Assert.That(logThatShouldNotBeChanged.AffectedUserEmail, Is.EqualTo("different@mail.de"));
        });
    }

    [Test]
    public async Task DeleteUser_InvalidUser_Test()
    {
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync((IdentityUser?)null);

        var result = await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);

        Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void DeleteUser_SelfDeletionAttempt_Test()
    {
        var user = new IdentityUser {Email = "camo", Id = "1"};

        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.GetUserByEmailAsync("camo")).ReturnsAsync(user);

        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None));
    }
}
