using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Plugins;
/// <summary>
/// 
/// </summary>
public class GetAllPluginsForProjectIdQueryHandler : IRequestHandler<GetAllPluginsForProjectIdQuery, IEnumerable<Plugin>>
{
    IPluginRepository _pluginRepository;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginRepository"></param>
    public GetAllPluginsForProjectIdQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<Plugin>> Handle(GetAllPluginsForProjectIdQuery request, CancellationToken cancellationToken)
    {
        return _pluginRepository.GetAllPluginsForProjectIdAsync(request.Id);
    }
}