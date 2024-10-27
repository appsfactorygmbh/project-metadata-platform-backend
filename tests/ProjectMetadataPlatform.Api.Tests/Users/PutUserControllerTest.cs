using System;
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
        _mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()));
        var request= new CreateUserRequest(  "Example Name", "Example Username", "Example Email", "Example Password");
        ActionResult result = await _controller.Put(1,request);
        Assert.That(result, Is.InstanceOf<StatusCodeResult>());

    }

    [Test]
    public async Task CreateUser_InvalidPassword_Test()
    {
        //prepare
        _mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ArgumentException("Invalid password"));
        var request= new CreateUserRequest(  "Example Name", "Example Username", "Example Email", "Example Password");
        ActionResult result = await _controller.Put(1,request);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid password"));
    }

}
