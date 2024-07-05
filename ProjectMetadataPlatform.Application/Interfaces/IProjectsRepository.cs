using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <param name="search">Search pattern to look for in ProjectName</param>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetProjectsAsync(string search);

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

    /// <summary>
    /// Asynchronously retrieves a collection of projects filtered by the specified business unit and/or team number.
    /// </summary>
    /// <param name="BusinessUnit">The business unit to filter projects by. Can be null to ignore this filter.</param>
    /// <param name="TeamNumber">The team number to filter projects by. Can be null to ignore this filter.</param>
    /// <returns>An Enumeration of projects</returns>
    Task<IEnumerable<Project>> GetBusinessUnitAndTeamNumberAsync(string? BusinessUnit, int? TeamNumber);
}
