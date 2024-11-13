using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.Users;

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
        _repository = new UsersRepository(_context, _mockUserManager.Object);

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
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var id = await _repository.CreateUserAsync(user, password);
        Assert.That(id, Is.EqualTo("1"));
    }

    [Test]
    public async Task CreateUserAsync_InvalidPassword_Test()
    {
        _context.Users.Add(new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email", Id = "1" });
        var user = new User { UserName = "Example Username", Name = "Example Name", Email = "Example Email" };
        var password = "test";
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

        Assert.ThrowsAsync<ArgumentException>(() => _repository.CreateUserAsync(user, password));
    }

    [Test]
    public async Task GetAllUsersAsync_EmptyResponse_Test()
    {
        var result = await _repository.GetAllUsersAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<User>>());
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetAllUsersAsync_Test()
    {
        var usersResponseContent = new List<User>
        {
            new()
            {
                Id = "1",
                Name = "Hinz"
            }
        };

        _context.Users.AddRange(usersResponseContent);
        _context.SaveChanges();

        var result = await _repository.GetAllUsersAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<User>>());
        Assert.Multiple((() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.ElementAt(0).Id, Is.EqualTo("1"));
            Assert.That(result.ElementAt(0).Name, Is.EqualTo("Hinz"));
        }));
    }

    [Test]
    public async Task GetUserByIdAsync_Test()
    {
        var user = new User
        {
            Id = "1",
            Name = "Hinz"
        };
        _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);

        var result = await _repository.GetUserByIdAsync("1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<User>());
        Assert.Multiple((() =>
        {
            Assert.That(result.Id, Is.EqualTo("1"));
            Assert.That(result.Name, Is.EqualTo("Hinz"));
        }));
    }

    [Test]
    public async Task GetUserByIdAsync_NonexistentUser_Test()
    {
        _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync((User?)null);

        var result = await _repository.GetUserByIdAsync("1");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetUserByUserNameAsync_Test()
    {
        var user = new User { UserName = "bigboss", Name = "Mr. Perkins", Email = "bigboss@bankofevil.com", Id = "1"};
        _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()) ).ReturnsAsync(user);
        var result = await _repository.GetUserByUserNameAsync("Example Username");
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task GetUserByUserNameAsync_NotFound_Test()
    {
        _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()) ).ReturnsAsync((User?)null);
        var result = await _repository.GetUserByUserNameAsync("Eiffel Tower (Vegas)");
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task StoreUser_CreatesUser_Test()
    {
        var user = new User { Id = "", Name = "Geordie Greep", UserName = "geordieCreep", Email = "notblackmidi@geordiegreep.com" };

        var result = await _repository.StoreUser(user);

        _mockUserManager.Verify(x => x.CreateAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task StoreUser_UpdatesUser_Test()
    {
        var user = new User { Id = "13", Name = "Linkin Park", UserName = "Clara Park", Email = "emily.armstrong@linkinpark.leipzig.de" };

        var result = await _repository.StoreUser(user);

        _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));
    }
    [Test]
    public async Task DeleteUserAsync_Test()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _repository.DeleteUserAsync(user);

        _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));

    }

    [Test]
    public async Task DeleteUser_Failed_Test()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed());

        Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteUserAsync(user));
    }

}
