using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Repository for accessing and managing project data in the database.
/// </summary>
public class ProjectsRepository : RepositoryBase<Project>, IProjectsRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public ProjectsRepository(ProjectMetadataPlatformDbContext dbContext) : base(dbContext)
    {
    }
    
    /// <summary>
    /// Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetAllProjectsAsync() =>
        await GetEverything().ToListAsync();
}
