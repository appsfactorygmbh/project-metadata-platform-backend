using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

public class GetGlobalPluginsQueryHandler : IRequestHandler<GetGlobalPluginsQuery, IEnumerable<Plugin>>
{
    private readonly IPluginRepository _pluginRepository;
    
    public GetGlobalPluginsQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }
    
    public Task<IEnumerable<Plugin>> Handle(GetGlobalPluginsQuery request, CancellationToken cancellationToken)
    {
        return _pluginRepository.GetGlobalPluginsAsync();
    }
}
