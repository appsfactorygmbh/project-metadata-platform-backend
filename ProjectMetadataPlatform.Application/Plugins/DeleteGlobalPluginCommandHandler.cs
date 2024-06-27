using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="CreatePluginCommand"/>
/// </summary>
public class DeleteGlobalPluginCommandHandler : IRequestHandler<DeleteGlobalPluginCommand, int>
{
    private readonly IPluginRepository _pluginRepository;
    
    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler"/>.
    /// </summary>
    /// <param name="pluginRepository"></param>
    public DeleteGlobalPluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }
    
    /// <summary>
    /// Creates a new Plugin with the given name
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<int> Handle(DeleteGlobalPluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id);
        if (request.Id == 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "PluginId can't be 0");
        }
        plugin.IsArchived = true;
        await _pluginRepository.StorePlugin(plugin);
        
        return plugin.Id;
    }
}
