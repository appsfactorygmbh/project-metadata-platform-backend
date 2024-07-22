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
    Task AddLogForCurrentUser(int  projectId, Action action, string changes);
}
