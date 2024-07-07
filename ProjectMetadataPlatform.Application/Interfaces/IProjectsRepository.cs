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
    ///     Saves a project to the database and returns it.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task AddOrUpdate(Project project);
}
