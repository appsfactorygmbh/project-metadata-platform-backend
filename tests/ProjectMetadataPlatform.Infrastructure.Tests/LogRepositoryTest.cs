using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class LogRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private LogRepository _loggingRepository;
    private Mock<IUsersRepository> _mockUserRepository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var identity = new GenericIdentity("camo", "test");
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext
        {
            User = contextUser
        };
        httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

        _mockUserRepository = new Mock<IUsersRepository>();

        _loggingRepository = new LogRepository(_context, httpContextAccessorMock.Object, _mockUserRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        ClearData(_context);
    }

    [Test]
    public async Task CreateLogTest()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client"
        };
        await _context.Projects.AddAsync(exampleProject);

        var user = new User
        {
            Id = "42",
            UserName = "camo",
            Email = "camo",
            Name = "some user"
        };
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = "Example Project",
                Property = "ProjectName"
            },
            new()
            {
                OldValue = "",
                NewValue = "Example Business Unit",
                Property = "BusinessUnit"
            },
            new()
            {
                OldValue = "",
                NewValue = "1",
                Property = "TeamNumber"
            },
            new()
            {
                OldValue = "",
                NewValue = "Example Department",
                Property = "Department"
            },
            new()
            {
                OldValue = "",
                NewValue = "Example Client",
                Property = "ClientName"
            }
        };

        _mockUserRepository.Setup(_ => _.GetUserByUserNameAsync("camo")).ReturnsAsync(user);

        await _loggingRepository.AddLogForCurrentUser( exampleProject.Id, Action.ADDED_PROJECT, logChanges);
        _context.SaveChanges();
        var dbLog = await _context.Logs.Include(log => log.User).Include(log => log.Project).Include(log => log.Changes)
            .FirstOrDefaultAsync()!;
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.ADDED_PROJECT));
            Assert.That(dbLog.Email, Is.EqualTo("camo"));
            Assert.That(dbLog.UserId, Is.EqualTo("42"));
            Assert.That(dbLog.User, Is.EqualTo(user));
            Assert.That(dbLog.ProjectId, Is.EqualTo(exampleProject.Id));
            Assert.That(dbLog.Project, Is.EqualTo(exampleProject));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(5));
        });
    }

    [Test]
    public async Task GetLogsForProject_Test()
    {
        var exampleLog = new Log
        {
            Id = 1,
            UserId = null,
            Email = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "", NewValue = "Example Project", Property = "ProjectName" }
            ]
        };

        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog }
        };
        exampleLog.Project = exampleProject;

        await _context.Projects.AddAsync(exampleProject);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForProject(301);

        Assert.That(logs, Has.Count.EqualTo(1));
        Assert.That(logs[0], Is.EqualTo(exampleLog));
    }

    [Test]
    public async Task GetLogsForProject_Test_NoLogsForProjectReturnsEmptyList()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = null
        };
        await _context.Projects.AddAsync(exampleProject);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForProject(301);

        Assert.That(logs, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetAllLogs_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            UserId = null,
            Email = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "", NewValue = "Example Project", Property = "ProjectName" }
            ]
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog1 }
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            UserId = null,
            Email = "someEmail",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "Example Project", NewValue = "Another Project", Property = "ProjectName" }
            ]
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog2 }
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetAllLogs();

        Assert.That(logs, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetLogsWithSearch_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            UserId = null,
            Username = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "", NewValue = "Example Project", Property = "ProjectName" }
            ]
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog1 }
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            UserId = null,
            Username = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "Example Project", NewValue = "Another Project", Property = "ProjectName" }
            ]
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog2 }
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsWithSearch("Another Project");

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.UserId, Is.Null);
            Assert.That(log.Username, Is.EqualTo("someUserName"));
            Assert.That(log.ProjectId, Is.EqualTo(302));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_PROJECT));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLogsWithSearch_SearchInStaticMessagePartMapsToActionProperty_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            UserId = null,
            Username = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "", NewValue = "Example Project", Property = "ProjectName" }
            ]
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog1 }
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            UserId = null,
            Username = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { OldValue = "Example Project", NewValue = "Another Project", Property = "ProjectName" }
            ]
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            BusinessUnit = "Example Business Unit",
            TeamNumber = 1,
            Department = "Example Department",
            ClientName = "Example Client",
            Logs = new List<Log> { exampleLog2 }
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsWithSearch("updated");

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.UserId, Is.Null);
            Assert.That(log.Username, Is.EqualTo("someUserName"));
            Assert.That(log.ProjectId, Is.EqualTo(302));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_PROJECT));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }
}
