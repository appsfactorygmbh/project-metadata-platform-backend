using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Plugins;

/// <summary>
/// The repository for plugins that handles the data access.
/// </summary>
public class PluginRepository : IPluginRepository
{   
    /// <summary>
    /// Constructor for the PluginRepository.
    /// </summary>
    /// <param name="context"></param>
    public PluginRepository(ProjectMetadataPlatformDbContext context)
    {
        _context = context;
    }
    
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Gets all plugins for a given project id from database.
    /// </summary>
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    public async Task<List<ProjectPlugins>> GetAllPluginsForProjectIdAsync(int id)
    {
        
        //implement database with EF core and send request to it from here
        
        return [.. _context.ProjectPluginsRelation.Where(rel => rel.ProjectId == id).Include(rel => rel.Plugin)];
    }
}