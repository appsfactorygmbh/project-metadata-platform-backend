using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class AuthRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private AuthRepository _repository;
    private Mock<UserManager<User>> _mockUserManager;


    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
            null, null, null, null, null, null, null, null);
        _context = DbContext();
        _repository = new AuthRepository(_context,_mockUserManager.Object);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        using ProjectMetadataPlatformDbContext context = DbContext();

        context.Database.EnsureDeleted();
    }

    [Test]
    public async Task CreateUser_Test()
    {
        var username = "test";
        var password = "test";
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()) ).ReturnsAsync(IdentityResult.Success);
        var id=await _repository.CreateUser(username, password);
        Assert.That(id, Is.Not.Null);


    }

    [Test]
    public async Task CheckLogin_Successful_Test()
    {
        var username = "test";
        var password = "test";

        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var result=await _repository.CheckLogin(username, password);
        Assert.That(result, Is.True);


    }

    [Test]
    public async Task CheckLogin_Unsuccessful_Test()
    {
        var username = "test";
        var password = "test";

        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        var result=await _repository.CheckLogin(username, password);
        Assert.That(result, Is.False);


    }

    [Test]
    public async Task StoreRefreshToken_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);

        await _repository.StoreRefreshToken(username,token);
        var result = _repository.GetIf(rt=>rt.User.UserName==username).FirstOrDefault();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo(token));


    }

    [Test]
    public async Task UpdateRefreshToken_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();
        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(username,"oldToken");
        var count = _repository.GetEverything().Count();
        _context.ChangeTracker.Clear();
        await _repository.UpdateRefreshToken(username,token);
        var result = _repository.GetIf(rt=>rt.User.UserName==username).FirstOrDefault();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo(token));
        Assert.That(_repository.GetEverything().Count(),Is.EqualTo(count));


    }

    [Test]
    public async Task CheckRefreshTokenExists_Successful_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(username,token);
        var result = await _repository.CheckRefreshTokenExists(username);
        Assert.That(result, Is.True);


    }

    [Test]
    public async Task CheckRefreshTokenExists_Unsuccessful_Test()
    {
        var username = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);

        var result = await _repository.CheckRefreshTokenExists(username);
        Assert.That(result, Is.False);


    }

    [Test]
    public async Task CheckRefreshTokenRequest_Successful_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(username,token);
        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.True);


    }

    [Test]
    public async Task CheckRefreshTokenRequest_UnsuccessfulNoToken_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","6");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);

        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.False);


    }

    [Test]
    public async Task CheckRefreshTokenRequest_UnsuccessfulExpired_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","0");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(username,token);
        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.False);


    }

    [Test]
    public async Task GetUserNameByRefreshToken_Test()
    {
        var username = "test";
        var token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS","0");
        _context.Users.Add(new User { UserName = username });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(username,token);
        var result = await _repository.GetUserNameByRefreshToken(token);
        Assert.That(result, Is.EqualTo(username));


    }


}
