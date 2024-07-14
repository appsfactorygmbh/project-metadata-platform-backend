using System;
using System.Security.Authentication;
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
public class RefreshTokenQueryHandlerTest
{
    private RefreshTokenQueryHandler _handler;
    private Mock<IAuthRepository> _mockAuthRepo;
    [SetUp]
    public void Setup()
    {
        _mockAuthRepo = new Mock<IAuthRepository>();
        _handler = new RefreshTokenQueryHandler(_mockAuthRepo.Object);
    }


    [Test]
    [Ignore("cant retrieve environment variables in test")]
    public async Task HandleRefreshTokenQueryHandler_ValidToken_Test()
    {
        _mockAuthRepo.Setup(m => m.CheckRefreshTokenRequest(It.IsAny<string>())).ReturnsAsync(true);
        _mockAuthRepo.Setup(m => m.GetUserNamebyRefreshToken(It.IsAny<string>())).ReturnsAsync("admin");
        var request = new RefreshTokenQuery("refreshToken");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<JwtTokens>());
    }

    [Test]
    public void HandleRefreshTokenQueryHandler_InvalidToken_Test()
    {
        _mockAuthRepo.Setup(m => m.CheckRefreshTokenRequest(It.IsAny<string>())).ReturnsAsync(false);
        var request = new RefreshTokenQuery("invalidrefreshToken");

        Assert.ThrowsAsync<AuthenticationException>(() => _handler.Handle(request, It.IsAny<CancellationToken>()));
    }

}
