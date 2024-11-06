using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using static System.DateTimeOffset;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Logs;

/// <summary>
///  Repository for creating and accessing logs.
/// </summary>
public class LogRepository : RepositoryBase<Log>, ILogRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    ///     initialising context and httpContextAccessor to provide user information
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="httpContextAccessor"></param>
    public LogRepository(ProjectMetadataPlatformDbContext dbContext, IHttpContextAccessor httpContextAccessor, IUsersRepository usersRepository) : base(dbContext)
    {
        _context = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _usersRepository = usersRepository;
    }

    /// <summary>
    ///     Adds new log into database.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    public async Task AddLogForCurrentUser(int projectId, Action action, List<LogChange> changes)
    {
        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown user";
        // User? user = await _usersRepository.GetUserByUserNameAsync(username);
        User? user = null;

        var log = new Log
        {
            Username = username,
            UserId = user?.Id,
            Action = action,
            ProjectId = projectId,
            TimeStamp = UtcNow,
            Changes = changes
        };
        var projects = await _context.Projects.Include(b => b.Logs).FirstAsync(pro => pro.Id == projectId);
        projects.Logs!.Add(log);
        _ = await _context.SaveChangesAsync();
    }
}
