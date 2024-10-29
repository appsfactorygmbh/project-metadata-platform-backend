using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class UsersRepositoryTest : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
            null, null, null, null, null, null, null, null);
        _context = DbContext();
        _repository = new UsersRepository(_context,_mockUserManager.Object);

        ClearData(_context);
    }
    private ProjectMetadataPlatformDbContext _context;
    private UsersRepository _repository;
    private Mock<UserManager<User>> _mockUserManager;


    [TearDown]
    public void TearDown()
    {
        using ProjectMetadataPlatformDbContext context = DbContext();

        context.Database.EnsureDeleted();
    }

    [Test]
    public async Task CreateUserAsync_Test()
    {
        var user = new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email", };
        var password = "test";
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()) ).ReturnsAsync(IdentityResult.Success);
        var id=await _repository.CreateUserAsync(user, password);
        Assert.That(id, Is.EqualTo("1"));
    }

    [Test]
    public async Task CreateUserAsync_InvalidPassword_Test()
    {
        _context.Users.Add(new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email", Id = "1"});
        var user = new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email"};
        var password = "test";
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()) ).ReturnsAsync(IdentityResult.Failed());

        Assert.ThrowsAsync<ArgumentException>(() => _repository.CreateUserAsync(user, password));
    }

    [Test]
    public async Task GetUserByIdAsync_Test()
    {
        _context.Users.Add(new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email", Id = "1"});
        var user = new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email", Id = "1"};
        _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()) ).ReturnsAsync(user);
        var result = await _repository.GetUserByIdAsync(1);
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task GetUserByIdAsync_Unsuccessful_Test()
    {
        _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()) ).ReturnsAsync((User?)null);
        var result = await _repository.GetUserByIdAsync(1);
        Assert.That(result, Is.Null);
    }


}
