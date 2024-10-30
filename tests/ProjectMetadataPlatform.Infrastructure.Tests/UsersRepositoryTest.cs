using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Users;
using Moq;
using Microsoft.AspNetCore.Identity;

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
}
