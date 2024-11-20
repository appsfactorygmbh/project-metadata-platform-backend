using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
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

    // TODO keep in sync with Action enum and the LogConverter in the Api project
    private static readonly Dictionary<Action, string> ActionMessages = new()
    {
        {Action.ADDED_PROJECT, "created a new project with properties: ,"},
        {Action.UPDATED_PROJECT, "updated project properties: set from to,"},
        {Action.ARCHIVED_PROJECT, "archived project"},
        {Action.UNARCHIVED_PROJECT, "unarchived project"},
        {Action.ADDED_PROJECT_PLUGIN, "added a plugin to project with properties: = ,"},
        {Action.UPDATED_PROJECT_PLUGIN, "updated a plugin in project: set from to , "},
        {Action.REMOVED_PROJECT_PLUGIN, "removed a plugin from project with properties: = ,"}
    };

    /// <summary>
    ///     initialising context and httpContextAccessor to provide user information
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="usersRepository"></param>
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
        User? user = await _usersRepository.GetUserByUserNameAsync(username);



        var log = new Log
        {
            Email = user?.Email,
            UserId = user?.Id,
            Action = action,
            ProjectId = projectId,
            TimeStamp = UtcNow,
            Changes = changes
        };
        _ = _context.Logs.Add(log);

    }

    /// <summary>
    ///     Adds new log into database. Uses the project object instead of the projectId.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    public async Task AddLogForCurrentUser(Project project, Action action, List<LogChange> changes)
    {
        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown user";
        User? user = await _usersRepository.GetUserByUserNameAsync(username);


        var log = new Log
        {
            Email = user?.Email,
            UserId = user?.Id,
            Action = action,
            Project = project,
            ProjectId = project.Id,
            TimeStamp = UtcNow,
            Changes = changes
        };
        _context.Logs.Add(log);
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsForProject(int projectId)
    {
        var projects = await _context.Projects.Include(b => b.Logs).FirstAsync(pro => pro.Id == projectId);

        return projects.Logs != null ? SortByTimestamp([.. projects.Logs]) : [];
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsWithSearch(string search)
    {
        var actionsToInclude = ActionMessages.Keys.Where(action => ActionMessages[action].Contains(search)).ToList();

        var res = _context.Logs
            .Include(l => l.Changes)
            .Include(l => l.Project)
            .Where(log =>
                (log.Email != null && log.Email.Contains(search))
                || actionsToInclude.Contains(log.Action)
                || (log.Project != null && log.Project.ProjectName.Contains(search))
                || (log.Changes != null && log.Changes.Any(change =>
                    change.Property.Contains(search)
                    || change.OldValue.Contains(search)
                    || change.NewValue.Contains(search))));

        return SortByTimestamp(await res.ToListAsync());
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetAllLogs()
    {
        return SortByTimestamp(await GetEverything()
            .Include(log => log.Project)
            .Include(log => log.User)
            .Include(log => log.Changes)
            .ToListAsync());
    }

    /// <summary>
    /// Sorts a list of logs by their timestamp.
    /// </summary>
    /// <param name="logs">The list of logs to be sorted.</param>
    /// <returns>A list of logs sorted by timestamp.</returns>
    private static List<Log> SortByTimestamp(List<Log> logs)
    {
        return [.. logs.OrderBy(log => log.TimeStamp)];
    }
}
