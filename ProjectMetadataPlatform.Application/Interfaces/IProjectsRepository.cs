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
    ///     Returns Projects based on ProjectName search string.
    /// </summary>
    /// <param name="search"> Search String for ProjectName</param>
    /// <returns>List of projects (may be empty)</returns>
    Task<IEnumerable<Project>?> GetProjectsProjectNameAsync(string search);

    /// <summary>
    ///     Returns Projects based on ClientName search string.
    /// </summary>
    /// <param name="search"> Search String for ClientName</param>
    /// <returns>List of projects (may be empty)</returns>
    Task<IEnumerable<Project>?> GetProjectsClientNameAsync(string search);

    /// <summary>
    ///     Returns a collection of projects that belong to the specified business units.
    /// </summary>
    /// <param name="businessUnits">A list of business units to filter the projects by.</param>
    /// <returns>An Enumeration of projects that belong to the specified business units.</returns>
    Task<IEnumerable<Project>> GetProjectsByBusinessUnitsAsync(List<string> businessUnits);

    /// <summary>
    /// Returns a collection of projects that belong to the specified team numbers.
    /// </summary>
    /// <param name="teamNumbers">A list of team numbers to filter the projects by.</param>
    /// <returns>An Enumeration of projects that belong to the specified team numbers.</returns>
    Task<IEnumerable<Project>> GetProjectsByTeamNumbersAsync(List<int> teamNumbers);


    /// <summary>
    ///     Saves a project to the database and returns it.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task AddOrUpdate(Project project);
}
