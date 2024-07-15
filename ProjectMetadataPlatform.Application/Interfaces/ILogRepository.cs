using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface ILogRepository
{

    /// <summary>
    ///     Adds new log for user.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="action"></param>
    /// <param name="changes"></param>
    /// <returns></returns>
    Task AddLogForCurrentUser(Project project, Action action, string changes);
}
