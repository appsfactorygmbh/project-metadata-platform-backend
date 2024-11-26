using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Api.Tests.Users;

public class GetMeControllerTest
{
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
    }

    [Test]
    public async Task getMe_Test()
    {
        var user = new User{Id = "42", Email = "moonstealer@gruhq.com"};

        _mediator.Setup(m => m.Send(It.IsAny<GetUserByUserNameQuery>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(user);
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor("moonstealer"));

        var result = await controller.GetMe();
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUserResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo("42"));
            Assert.That(response.Email, Is.EqualTo("moonstealer@gruhq.com"));
        });
    }

    [Test]
    public async Task getMe_Test_NotFound()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetUserByUserNameQuery>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((User)null!);
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor("moonstealer"));

        var result = await controller.GetMe();
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task getMe_Test_InternalError()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetUserByUserNameQuery>(), It.IsAny<System.Threading.CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Internal error"));
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor("Dr. Nefario"));

        var result = await controller.GetMe();
        var objectResult = result.Result as StatusCodeResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        });
    }

    [Test]
    public async Task getMe_Test_Unauthorized()
    {
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor(null));

        var result = await controller.GetMe();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
        Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
    }

    private static HttpContextAccessor MockHttpContextAccessor(string? username)
    {
        if (username == null)
        {
            return new HttpContextAccessor
            {
                HttpContext = null
            };
        }
        var claims = new System.Collections.Generic.List<Claim> { new(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        return new HttpContextAccessor
        {
            HttpContext = httpContext
        };
    }
}
