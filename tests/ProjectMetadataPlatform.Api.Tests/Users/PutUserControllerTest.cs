using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Projects;
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
        Assert.That(result.GetType(), Is.InstanceOf<CreatedResult>());
        _mediator.Verify(mediator => mediator.Send(It.Is<CreateUserCommand>(command =>
                command.UserId == 1 && command.Name == "Example Name" && command.Username == "Example Username" &&
                command.Email == "Example Email" && command.Password == "Example Password"),
            It.IsAny<CancellationToken>()));
    }

}
