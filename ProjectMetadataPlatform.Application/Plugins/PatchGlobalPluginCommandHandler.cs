using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handles the PatchGlobalPluginCommand request.
/// </summary>
public class PatchGlobalPluginCommandHandler : IRequestHandler<PatchGlobalPluginCommand, Plugin?>
{
    private readonly IPluginRepository _pluginRepository;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PatchGlobalPluginCommandHandler"/> class.
    /// </summary>
    /// <param name="pluginRepository">The plugin repository to use for plugin operations.</param>
    public PatchGlobalPluginCommandHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    /// <summary>
    /// Handles the PatchGlobalPluginCommand request.
    /// </summary>
    /// <param name="request">The PatchGlobalPluginCommand request to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Plugin that was updated.</returns>
    public async Task<Plugin?> Handle(PatchGlobalPluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id);
        
        if (plugin == null)
        {
            return null;
        }
        
        plugin.PluginName = request.PluginName ?? plugin.PluginName;
        plugin.IsArchived = request.IsArchived ?? plugin.IsArchived;

        return await _pluginRepository.StorePlugin(plugin);
    }
}