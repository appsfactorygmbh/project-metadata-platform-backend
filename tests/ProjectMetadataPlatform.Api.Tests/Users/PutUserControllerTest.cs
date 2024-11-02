using System;
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


namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class PutUserControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object);
    }
    private UsersController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public async Task CreateUser_Test()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("1");
        var request= new CreateUserRequest(  "Example Username", "Example Name", "Example Email", "Example Password");
        ActionResult<CreateUserResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateUserResponse>());

        var userResponse = createdResult.Value as CreateUserResponse;
        Assert.That(userResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userResponse.UserId, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Users/1"));
        });
        _mediator.Verify(mediator => mediator.Send(It.Is<CreateUserCommand>(command =>
                command.Username == "Example Username" && command.Name == "Example Name" &&
                command.Email == "Example Email" && command.Password == "Example Password"),
            It.IsAny<CancellationToken>()));

    }

    [Test]
    public async Task CreateUser_InvalidPassword_Test()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ArgumentException("Invalid password"));
        var request= new CreateUserRequest(  "Example Name", "Example Username", "Example Email", "Example Password");
        ActionResult<CreateUserResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid password"));
    }

    [Test]
    public async Task CreateUser_InvalidRequest_Test()
    {
        var request= new CreateUserRequest(  "", "", "", "");
        ActionResult<CreateUserResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("name, username, email and password must not be empty."));
    }

    [Test]
    public async Task CreateUser_MediatorThrowsOtherExceptionTest()
    {
        _mediator.Setup(mediator => mediator.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("An error message"));
        var request= new CreateUserRequest(  "Example Name", "Example Username", "Example Email", "Example Password");
        ActionResult<CreateUserResponse> result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());

        var badRequestResult = result.Result as StatusCodeResult;
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(500));
    }

}
