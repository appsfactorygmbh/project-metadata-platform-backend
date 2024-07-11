using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Auth;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Tests.Auth;

public class Tests
{
    private AuthController _controller;
    private Mock<IMediator> _mediator;
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new AuthController(_mediator.Object);
    }

    [Test]
    public async Task SuccessfulLoginTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JwtTokens{AccessToken = "accessToken", RefreshToken = "refreshToken"});

        var request = new LoginRequest("username", "password");

        var result = await _controller.Put(request);
        Assert.That(result.Value, Is.InstanceOf<LoginResponse>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.accessToken, Is.EqualTo("accessToken"));
            Assert.That(result.Value.refreshToken, Is.EqualTo("refreshToken"));
        });
        ;
    }

    [Test]
    public async Task WrongCredentialsLoginTest()
    {
        _mediator.Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid login credentials."));

        var request = new LoginRequest("username", "password");

        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult!.Value, Is.EqualTo("Invalid login credentials."));
    }
}
