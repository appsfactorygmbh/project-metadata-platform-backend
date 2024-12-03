using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using System.IO;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class GetUserControllerTest
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
    public async Task Get_ReturnsUser()
    {
        var user = new IdentityUser
        {
            Id = "1",
            Email = "Hinz"
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _controller.GetUserById("1");

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUserResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple((() =>
        {
            Assert.That(response.Id, Is.EqualTo("1"));
            Assert.That(response.Email, Is.EqualTo("Hinz"));
        }));
    }

    [Test]
    public async Task GetUserById_NonexistentUser_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetUserQuery>(q => q.UserId == "1"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUser?)null);
        ActionResult<GetUserResponse> result = await _controller.GetUserById("1");
        Assert.That(result, Is.Not.Null);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value, Is.EqualTo("No User with id 1 was found."));
    }

    [Test]
    public async Task MediatorThrowsExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var result = await _controller.GetUserById("1");
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }
}
