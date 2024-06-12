using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Interfaces;


/// <summary>
/// Repository for plugins 
/// </summary>
public interface IPluginRepository
{
    /// <summary>
    /// Returns a collection of plugins for a given project id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<ProjectPlugins>> GetAllPluginsForProjectIdAsync(int id);
    
    /// <summary>
    /// Creates a new Plugin with the given name
    /// </summary>
    /// <param name="name">The name of the new Plugin</param>
    /// <returns></returns>
    Task<Plugin> CreatePlugin(string name);
}