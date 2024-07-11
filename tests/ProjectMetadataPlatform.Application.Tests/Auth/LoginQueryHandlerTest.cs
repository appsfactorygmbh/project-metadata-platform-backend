using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class LoginQueryHandlerTest
{
    private LoginQueryHandler _handler;
    private Mock<IAuthRepository> _mockAuthRepo;
    [SetUp]
    public void Setup()
    {
        _mockAuthRepo = new Mock<IAuthRepository>();
        _handler = new LoginQueryHandler(_mockAuthRepo.Object);
    }

    [Test]
    public async Task HandleLoginQueryHandler_ValidLogin_Test()
    {
        _mockAuthRepo.Setup(m => m.CheckLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var request = new LoginQuery("username", "password");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<JwtTokens>());
    }

    [Test]
    public void HandleLoginQueryHandler_InvalidLogin_Test()
    {
        _mockAuthRepo.Setup(m => m.CheckLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var request = new LoginQuery("username", "password");

        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, It.IsAny<CancellationToken>()));
    }
}
