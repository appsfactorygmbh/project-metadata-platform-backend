using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Infrastructure.Plugins;

/// <summary>
/// 
/// </summary>

public class PluginRepository : IPluginRepository
{
    public Task<IEnumerable<Plugin>> GetAllPluginsForProjectIdAsync(int projectId)
    {
        //implement database with EF core and send request to it from here
        var result = new List<Plugin>
        {
            new Plugin { Id = 1, PluginName = "Plugin 1", Url = "https://plugin1.com" },
            new Plugin { Id = 2, PluginName = "Plugin 2", Url = "https://plugin2.com" },
            new Plugin { Id = 3, PluginName = "Plugin 3", Url = "https://plugin3.com" }
        }.AsEnumerable();
        return Task.FromResult(result);
    }
}