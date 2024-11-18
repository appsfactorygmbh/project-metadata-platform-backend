using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
///     Repository for all projects or one specific project with an id.
/// </summary>
public interface IProjectsRepository
{
    /// <summary>
    ///     Returns a collection of all projects with specific search pattern.
    /// </summary>
    /// <param name="query">The query containing filters and search pattern.</param>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetProjectsAsync(GetAllProjectsQuery query);

    /// <summary>
    ///     Returns a collection of all projects.
    /// </summary>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetProjectsAsync();

    /// <summary>
    ///     Returns a project.
    /// </summary>
    /// <returns>One project or null.</returns>
    Task<Project?> GetProjectAsync(int id);

    /// <summary>
    ///     Returns a project.
    /// </summary>
    /// <returns>One project or null.</returns>
    Task<Project?> GetProjectWithPluginsAsync(int id);

    /// <summary>
    ///     Saves a project to the database.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task Add(Project project);

    /// <summary>
    /// Checks if a project exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True, if the project with the given id exists</returns>
    Task<bool> CheckProjectExists(int id);

    /// <summary>
    /// Asynchronously retrieves a distinct list of business units from all projects.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, which upon completion returns a collection of distinct business unit names.</returns>
    Task<IEnumerable<string>> GetBusinessUnitsAsync();

    /// <summary>
    /// Asynchronously retrieves a distinct list of team numbers from all projects.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, which upon completion returns a collection of distinct team numbers.</returns>
    Task<IEnumerable<int>> GetTeamNumbersAsync();
}
