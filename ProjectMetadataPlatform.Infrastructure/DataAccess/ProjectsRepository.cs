using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
///     Repository for accessing and managing project data in the database.
/// </summary>
public class ProjectsRepository : RepositoryBase<Project>, IProjectsRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public ProjectsRepository(ProjectMetadataPlatformDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    /// <summary>
    ///     Asynchronously retrieves all projects with specific search pattern from the database.
    /// </summary>
    /// ///
    /// <param name="search">Search pattern to look for in ProjectName</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync(string search)
    {
            return [.. _context.Projects.Where(project => project.ProjectName.Contains(search) 
                                                          || project.ClientName.Contains(search)
                                                          || project.BusinessUnit.Contains(search)
                                                          || project.Department.Contains(search)
            )];
    }

    /// <summary>
    ///     Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        return await GetEverything().ToListAsync();
    }


    /// <summary>
    ///     Asynchronously retrieves a project from the database by its identifier.
    /// </summary>
    /// <param name="id">Identification number for a project</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns one project.</returns>
    public async Task<Project?> GetProjectAsync(int id)
    {
        return await GetIf(p => p.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    ///     Saves project to the database and returns it.
    /// </summary>
    /// <param name="project">Project to be saved in the database</param>
    /// <returns>Project is returned</returns>
    public async Task Add(Project project)
    {
        if (GetIf(p => p.Id == project.Id).FirstOrDefault() == null)
        {
            Create(project);
        }
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates a project in the database and returns it.
    /// </summary>
    /// <param name="project">Project to be updated</param>
    /// <param name="plugins">Plugins of the project</param>
    /// <returns></returns>
    public async Task UpdateProject(Project project,List<ProjectPlugins> plugins)
    {
        _context.ProjectPluginsRelation.AddRange(plugins);
        if(GetIf(p => p.Id == project.Id).FirstOrDefault() != null)
        {
            var existingProject = await _context.Projects
                .Include(p => p.ProjectPlugins)
                .FirstOrDefaultAsync(p => p.Id == project.Id);
            existingProject!.ProjectName = project.ProjectName;
            existingProject.Department = project.Department;
            existingProject.BusinessUnit = project.BusinessUnit;
            existingProject.TeamNumber = project.TeamNumber;
            existingProject.ClientName = project.ClientName;
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Deletes all plugins associated with a project.
    /// </summary>
    /// <param name="id">The id of the project from which all associated plugins will be deleted</param>
    public async Task DeletePluginAssociation(int id)
    {
        _context.ProjectPluginsRelation.RemoveRange(_context.ProjectPluginsRelation.Where(p => p.ProjectId == id));
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a project exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True, if the project with the given id exists</returns>
    public Task<bool> CheckProjectExists(int id)
    {
        return GetIf(p => p.Id == id).AnyAsync();
    }


}
