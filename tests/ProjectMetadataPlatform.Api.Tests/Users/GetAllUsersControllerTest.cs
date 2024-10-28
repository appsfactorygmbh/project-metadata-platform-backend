using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Api.Users.Models;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class GetAllUsersControllerTest
{
    private UsersController _controller;
    public Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object);
    }

    [Test]
    public async Task Get_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new User
            {
                Id = "1",
                Name = "Hinz"
            },
            new User
            {
                Id = "2",
                Name = "Kunz"
            }
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<GetAllUsersResponse>;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple((() =>
        {
            Assert.That(response.Count(), Is.EqualTo(2));
            Assert.That(response.ElementAt(0).Id, Is.EqualTo("1"));
            Assert.That(response.ElementAt(0).Name, Is.EqualTo("Hinz"));
            Assert.That(response.ElementAt(1).Id, Is.EqualTo("2"));
            Assert.That(response.ElementAt(1).Name, Is.EqualTo("Kunz"));
        }));
    }

    [Test]
    public async Task Get_ReturnsEmptyList()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<GetAllUsersResponse>;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count(), Is.EqualTo(0));
    }

}
