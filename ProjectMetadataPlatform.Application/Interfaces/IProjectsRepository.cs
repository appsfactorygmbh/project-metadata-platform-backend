using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for all projects or one specific project with an id.
/// </summary>

public interface IProjectsRepository
{
    /// <summary>
    /// Returns a collection of all projects.
    /// </summary>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetAllProjectsAsync();
   
    /// <summary>
    /// Returns a project.
    /// </summary>
    /// <returns>One project or null.</returns>
    Task<Project?> GetProjectAsync(int id);
    
    /// <summary>
    /// Creates a new Project with the given attributes.
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="businessUnit">Name of the business unit</param>
    /// <param name="teamNumber">Number of the team</param>
    /// <param name="department">Name of the department</param>
    /// <param name="clientName">Name of the client</param>
    /// <returns>creates Project</returns>
    Task<Project> CreateProject(string projectName, string businessUnit, int teamNumber, string department, string clientName);
    
    /// <summary>
    /// Saves a project to the database and returns it.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task<Project> Updatewithreturnvalue(Project project);
}