using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
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
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var identity = new GenericIdentity("camo", "test");
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext() {
            User = contextUser
        };
        httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);
        _loggingRepository = new LogRepository(_context, httpContextAccessorMock.Object);

        _context = DbContext();
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
        await _loggingRepository.AddLogForCurrentUser( exampleProject.Id, Action.Added, "added project");
        var dbLog = await _context?.Logs.FirstOrDefaultAsync()!;
        Assert.NotNull(dbLog);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Action, Is.EqualTo(Action.Added));
            Assert.That(dbLog.Username, Is.EqualTo("camo"));
        });
    }
}
