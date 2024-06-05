using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Infrastructure.Plugins;

/// <summary>
/// The repository for plugins that handles the data access.
/// </summary>
public class PluginRepository : IPluginRepository
{
    /// <summary>
    /// Gets all plugins for a given project id from database.
    /// </summary>
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    public Task<IEnumerable<Plugin>> GetAllPluginsForProjectIdAsync(int id)
    {
        //implement database with EF core and send request to it from here
        var result = new List<Plugin>
        {
            new() { Id = 1, PluginName = "Plugin 1", Url = "https://plugin1.com" },
            new() { Id = 2, PluginName = "Plugin 2", Url = "https://plugin2.com" },
            new() { Id = 3, PluginName = "Plugin 3", Url = "https://plugin3.com" }
        }.AsEnumerable();
        return Task.FromResult(result);
    }
}