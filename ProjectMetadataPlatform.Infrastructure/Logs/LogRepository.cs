using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Logs;

public class LogRepository : RepositoryBase<Log>, ILogRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogRepository(ProjectMetadataPlatformDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
    {
        _context = dbContext;
        this._httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Adds new log into database.
    /// </summary>
    /// <param name="log"></param>
    public async Task AddLogForCurrentUser(Project project, Action action, string changes)
    {
        string username = _httpContextAccessor.HttpContext.User.Identity!.Name;
        Log log = new Log
        {
            Username = username,
            Action = action,
            Changes = changes,
            Project = project,
            ProjectId = project.Id,
            TimeStamp = DateTimeOffset.UtcNow
        };
        Create(log);
        _ = await _context.SaveChangesAsync();
    }

}
