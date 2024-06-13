using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using System.IO;
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
    /// Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetAllProjectsAsync() =>
        await GetEverything().ToListAsync();
   
    /// <summary>
    /// Asynchronously retrieves a project from the database by its identifier.
    /// </summary>
    /// <param name="id">Identification number for a project</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns one project.</returns>
    public async Task<Project?> GetProjectAsync(int id) =>
        await GetIf(p => p.Id == id).FirstOrDefaultAsync(); 
   
    /// <summary>
    /// Creates a new Project in the database.
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="businessUnit">Name of the business unit</param>
    /// <param name="teamNumber">Number of the team</param>
    /// <param name="department">Name of the department</param>
    /// <param name="clientName">Name of the client</param>
    /// <returns>created project</returns>
    /// <exception cref="IOException"></exception>
    public async Task<Project> CreateProject(string projectName, string businessUnit, int teamNumber, string department, string clientName)
    {
        var project = new Project
        {
            ProjectName = projectName,
            BusinessUnit = businessUnit,
            TeamNumber = teamNumber,
            Department = department,
            ClientName = clientName
        };
        var savedProject= _context.Projects.Add(project).Entity;
        var savedEntries = await _context.SaveChangesAsync();
        if (savedEntries == 0)
        {
            throw new IOException("Project could not be saved.");
        }

        return savedProject;
    }
}
