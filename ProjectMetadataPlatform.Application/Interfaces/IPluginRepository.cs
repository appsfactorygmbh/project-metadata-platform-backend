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
}