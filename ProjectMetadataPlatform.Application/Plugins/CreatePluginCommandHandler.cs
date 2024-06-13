using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="CreatePluginCommand"/>
/// </summary>
public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, int>
{
    private readonly IPluginRepository _pluginRepository;
    
    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler"/>.
    /// </summary>
    /// <param name="pluginRepository"></param>
    public CreatePluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }
    
    /// <summary>
    /// Creates a new Plugin with the given name
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<int> Handle(CreatePluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = new Plugin { PluginName = request.Name, ProjectPlugins = []};
        var storedPlugin = await _pluginRepository.StorePlugin(plugin);

        return storedPlugin.Id;
    }
}
