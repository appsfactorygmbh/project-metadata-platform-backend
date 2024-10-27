using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.User;


namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class CreateUserCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _handler = new CreateUserCommandHandler(_mockUsersRepo.Object);
    }
    private CreateUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;

    [Test]
    public async Task CreateUser_Test()
    {
        var exampleUser = new User { Name = "Example Name", UserName = "Example Username", Email = "Example Email", Id = "1"};
        _mockUsersRepo.Setup(m => m.CreateUserAsync(It.IsAny<User>(),It.IsAny<string>())).ReturnsAsync("1");

        var result = await _handler.Handle(new CreateUserCommand(1,"Example Username", "Example Name", "Example Email", "Example Password"), It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo("1"));
    }



}
