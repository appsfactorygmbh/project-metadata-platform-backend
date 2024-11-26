using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Api.Tests.Users;

public class PatchUsersControllerTest
{
    private UsersController _controller;
    private Mock<IMediator> _mediator;
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object, null!);
    }

    [Test]
    public async Task PatchUser_Test()
    {
        var user = new User
        {
            Id = "42",
            Email = "dr@core.fr",
            PasswordHash = "someHash"
        };

        _mediator.Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var request = new PatchUserRequest(null, "Dr. Peacock");

        ActionResult<GetUserResponse> result = await _controller.Patch("42", request);
        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as GetUserResponse;

        Assert.Multiple(() =>
        {
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue.Email, Is.EqualTo("dr@core.fr"));
            Assert.That(resultValue.Id, Is.EqualTo("42"));
        });
    }

    [Test]
    public async Task PatchUser_NotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null!);

        var request = new PatchUserRequest(null, "Black Midi");

        ActionResult<GetUserResponse> result = await _controller.Patch("404", request);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("No user with id 404 was found."));
    }

    [Test]
    public async Task PatchUser_InternalError_Test()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.Patch("13", new PatchUserRequest(null, "The Smiths"));
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }
}
