using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using System.Linq;
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
        _context = dbContext;
    }
    private readonly ProjectMetadataPlatformDbContext _context;
    
    /// <summary>
    /// Asynchronously retrieves all projects with specific search pattern from the database.
    /// </summary>
    /// /// <param name="search">Search pattern to look for in ProjectName</param>
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
    /// Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        return await GetEverything().ToListAsync();
    }
       
   
    /// <summary>
    /// Asynchronously retrieves a project from the database by its identifier.
    /// </summary>
    /// <param name="id">Identification number for a project</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns one project.</returns>
    public async Task<Project?> GetProjectAsync(int id) =>
        await GetIf(p => p.Id == id).FirstOrDefaultAsync(); 
    
    /// <summary>
    /// Saves project to the database and returns it.
    /// </summary>
    /// <param name="project">Project to be saved in the database</param>
    /// <returns>Project is returned</returns>
    public async Task AddOrUpdate(Project project)
    {
        if(GetIf(p => p.Id == project.Id).FirstOrDefault() == null)
        {
            Create(project);
            
        }
        else
        {
            Update(project);
        }
        await _context.SaveChangesAsync();
        
    }
}
