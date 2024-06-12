using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="CreatePluginCommand"/>
/// </summary>
public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, Plugin>
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
    /// Handles the request to get all plugins for a given project id.
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public Task<Plugin> Handle(CreatePluginCommand request, CancellationToken cancellationToken)
    {
        return  _pluginRepository.CreatePlugin(request.name);
    }
}
