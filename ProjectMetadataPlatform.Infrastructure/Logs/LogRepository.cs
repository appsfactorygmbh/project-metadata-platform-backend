using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using static System.DateTimeOffset;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Logs;

public class LogRepository : RepositoryBase<Log>, ILogRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    private readonly HttpContext _httpContextAccessor;

    public LogRepository(ProjectMetadataPlatformDbContext dbContext, HttpContext httpContextAccessor) : base(dbContext)
    {
        _context = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Adds new log into database.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="project"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    public async Task AddLogForCurrentUser(Project project, Action action, string changes)
    {
        var username = _httpContextAccessor.User.Identity.Name;
        var log = new Log
        {
            Username = username ?? "admin",
            Action = action,
            Changes = changes,
            Project = project,
            ProjectId = project.Id,
            TimeStamp = UtcNow
        };
        var projects = await _context.Projects.Include(b => b.Logs).FirstAsync(pro => pro.Id == project.Id);
        projects.Logs!.Add(log);
        _ = await _context.SaveChangesAsync();
    }
}
