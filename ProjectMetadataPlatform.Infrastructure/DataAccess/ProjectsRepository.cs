using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

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
    ///     Asynchronously retrieves all projects with specific search pattern or filter matches from the database.
    /// </summary>
    /// ///
    /// <param name="query">The query containing filters and search pattern.</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync(GetAllProjectsQuery query)
    {
        var filteredQuery = _context.Projects.AsQueryable();

        if(!string.IsNullOrWhiteSpace(query.Search))
        {
            var lowerTextSearch = query.Search.ToLower();
            filteredQuery = filteredQuery.Where(project => project.ProjectName.ToLower().Contains(lowerTextSearch)
                                                  || project.ClientName.ToLower().Contains(lowerTextSearch)
                                                  || project.BusinessUnit.ToLower().Contains(lowerTextSearch)
                                                  || project.TeamNumber.ToString().Contains(lowerTextSearch));
        }

        if (query.Request != null)
        {
            if (!string.IsNullOrWhiteSpace(query.Request.ProjectName))
            {
                var lowerProjectNameSearch = query.Request.ProjectName.ToLower();
                filteredQuery = filteredQuery.Where(project =>
                    project.ProjectName.ToLower().Contains(lowerProjectNameSearch)
                );
            }

            if(!string.IsNullOrWhiteSpace(query.Request.ClientName))
            {
                var lowerClientNameSearch = query.Request.ClientName.ToLower();
                filteredQuery = filteredQuery.Where(project =>
                    project.ClientName.ToLower().Contains(lowerClientNameSearch)
                );
            }

            if (query.Request.BusinessUnit is { Count: > 0 })
            {
                var lowerBusinessUnits = query.Request.BusinessUnit.Select(bu => bu.ToLower()).ToList();
                filteredQuery = filteredQuery.Where(project =>
                    lowerBusinessUnits.Contains(project.BusinessUnit.ToLower())
                );
            }

            if (query.Request.TeamNumber is { Count: > 0 })
            {
                filteredQuery = filteredQuery.Where(project =>
                    query.Request.TeamNumber.Contains(project.TeamNumber)
                );
            }
        }

        return await filteredQuery.ToListAsync();
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
        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates a project in the database and returns it.
    /// </summary>
    /// <param name="project">Project to be updated</param>
    /// <param name="plugins">Plugins of the project</param>
    /// <returns></returns>
    public async Task UpdateProject(Project project,List<ProjectPlugins> plugins)
    {
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
            existingProject.ProjectPlugins = plugins;
            _ = await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Checks if a project exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True, if the project with the given id exists</returns>
    public async Task<bool> CheckProjectExists(int id)
    {
        return _context.Projects.Any(project => project.Id == id);
    }

    public async Task<IEnumerable<int>> GetTeamNumbersAsync()
    {
        return await _context.Projects.Select(project => project.TeamNumber).Distinct().ToListAsync();
    }
}
