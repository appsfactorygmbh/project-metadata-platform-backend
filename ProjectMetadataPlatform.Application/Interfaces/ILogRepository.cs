using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
///     Repository for logging project changes
/// </summary>
public interface ILogRepository
{

    /// <summary>
    ///     Adds new log for user.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    /// <returns></returns>
    Task AddLogForCurrentUser(int  projectId, Action action, List<LogChange> changes);

    /// <summary>
    ///     Adds new log for user. Uses a project object instead of a project id.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    /// <returns></returns>
    Task AddLogForCurrentUser(Project  project, Action action, List<LogChange> changes);

    /// <summary>
    /// Retrieves the logs for a specific project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <returns>A list of logs associated with the specified project.</returns>
    Task<List<Log>> GetLogsForProject(int projectId);

    /// <summary>
    /// Retrieves logs that match the specified search term.
    /// </summary>
    /// <param name="search">The search term to filter logs.</param>
    /// <returns>A list of logs that match the search term.</returns>
    Task<List<Log>> GetLogsWithSearch(string search);

    /// <summary>
    /// Retrieves all logs from the database.
    /// </summary>
    /// <returns>A list of all logs, sorted by timestamp.</returns>
    Task<List<Log>> GetAllLogs();
}
