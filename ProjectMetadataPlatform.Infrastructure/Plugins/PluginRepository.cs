using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Plugins;

/// <summary>
///     The repository for plugins that handles the data access.
/// </summary>
public class PluginRepository : IPluginRepository
{

    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Constructor for the PluginRepository.
    /// </summary>
    /// <param name="context"></param>
    public PluginRepository(ProjectMetadataPlatformDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Gets all plugins for a given project id from database.
    /// </summary>
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    public async Task<List<ProjectPlugins>> GetAllPluginsForProjectIdAsync(int id)
    {
        return [.. _context.ProjectPluginsRelation.Where(rel => rel.ProjectId == id).Include(rel => rel.Plugin)];
    }

    /// <summary>
    ///     Saves a given Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns>The saved Plugin</returns>
    public async Task<Plugin> StorePlugin(Plugin plugin)
    {
        _context.Plugins.Add(plugin);
        await _context.SaveChangesAsync();

        return plugin;
    }
}
