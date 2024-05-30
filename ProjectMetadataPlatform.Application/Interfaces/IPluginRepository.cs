using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IPluginRepository
{
    Task<IEnumerable<Plugin>> GetAllPluginsForProjectIdAsync(int id);
}