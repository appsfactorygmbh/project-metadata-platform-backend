using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="DeleteGlobalPluginCommand"/>
/// </summary>
public class DeleteGlobalPluginCommandHandler : IRequestHandler<DeleteGlobalPluginCommand, Plugin>
{
    private readonly IPluginRepository _pluginRepository;
    
    /// <summary>
    /// Creates a new instance of<see cref="DeleteGlobalPluginCommandHandler"/>.
    /// </summary>
    /// <param name="pluginRepository"></param>
    public DeleteGlobalPluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }
    
    /// <summary>
    /// Delete Plugin with the given id
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<Plugin> Handle(DeleteGlobalPluginCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            throw new ArgumentException("Plugin not found");
        }
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id);
        if (plugin == null)
        {
            throw new NullReferenceException("Plugin not found");
        }
        plugin.IsArchived = true;
        await _pluginRepository.StorePlugin(plugin);
        
        return plugin;
    }
}
