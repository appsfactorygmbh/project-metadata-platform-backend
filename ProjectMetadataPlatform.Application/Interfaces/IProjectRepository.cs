using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for all projects.
/// </summary>

public interface IProjectRepository
{
    /// <summary>
    /// Returns a collection of all projects.
    /// </summary>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetAllProjectsAsync();
}