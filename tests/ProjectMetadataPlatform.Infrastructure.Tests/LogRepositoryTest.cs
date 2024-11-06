using System.Collections.Generic;
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
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Logs;

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

        await _loggingRepository.AddLogForCurrentUser( exampleProject.Id, Action.ADDED_PROJECT, logChanges);
        var dbLog = await _context.Logs.Include(log => log.User).Include(log => log.Project).Include(log => log.Changes)
            .FirstOrDefaultAsync()!;
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.ADDED_PROJECT));
            Assert.That(dbLog.Username, Is.EqualTo("camo"));
            Assert.That(dbLog.UserId, Is.Null); // TODO change to actual user id once the user is set by the log repo
            Assert.That(dbLog.User, Is.Null);
            Assert.That(dbLog.ProjectId, Is.EqualTo(exampleProject.Id));
            Assert.That(dbLog.Project, Is.EqualTo(exampleProject));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(5));
        });
    }
}
