using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class DeleteUserControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object, null!);
    }
    private UsersController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new IdentityUser { Id = "1", Email = "John" };
        _mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);;
        ActionResult result = await _controller.Delete("1");
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mediator.Verify(mediator => mediator.Send(It.Is<DeleteUserCommand>(command => command.Id == "1"), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task DeleteUser_NotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync((IdentityUser?)null);
        ActionResult result = await _controller.Delete("1");
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        _mediator.Verify(mediator => mediator.Send(It.Is<DeleteUserCommand>(command => command.Id == "1"), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task DeleteUser_InternalError_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        var result = await _controller.Delete("");

        var objectResult = result as StatusCodeResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        });
    }

    [Test]
    public async Task DeleteUser_UserSelfDeletionAttempt_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("A User can't delete themself."));

        var result = await _controller.Delete("1");

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        _mediator.Verify(mediator => mediator.Send(It.Is<DeleteUserCommand>(command => command.Id == "1"), It.IsAny<CancellationToken>()));

    }

}
