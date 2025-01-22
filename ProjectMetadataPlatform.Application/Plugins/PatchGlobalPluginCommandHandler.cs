using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handles the PatchGlobalPluginCommand request.
/// </summary>
public class PatchGlobalPluginCommandHandler : IRequestHandler<PatchGlobalPluginCommand, Plugin>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchGlobalPluginCommandHandler"/> class.
    /// </summary>
    /// <param name="pluginRepository">The plugin repository to use for plugin operations.</param>
    /// <param name="logRepository">The log repository to use for logging operations.</param>
    /// <param name="unitOfWork">The unit of work to use for transactional operations.</param>
    public PatchGlobalPluginCommandHandler(IPluginRepository pluginRepository, ILogRepository logRepository,
        IUnitOfWork unitOfWork)
    {
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }


    /// <summary>
    /// Handles the PatchGlobalPluginCommand request.
    /// </summary>
    /// <param name="request">The PatchGlobalPluginCommand request to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Plugin that was updated.</returns>
    public async Task<Plugin> Handle(PatchGlobalPluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id) ?? throw new PluginNotFoundException(request.Id);
        if (request.PluginName != null && !string.Equals(plugin.PluginName, request.PluginName, StringComparison.Ordinal))
        {
            await AddUpdatedPluginNameLog(plugin, request);
            plugin.PluginName = request.PluginName;
        }

        if (request.IsArchived != null && plugin.IsArchived != request.IsArchived.Value)
        {
            plugin.IsArchived = request.IsArchived.Value;
            await _logRepository.AddGlobalPluginLogForCurrentUser(plugin, plugin.IsArchived ? Action.ARCHIVED_GLOBAL_PLUGIN : Action.UNARCHIVED_GLOBAL_PLUGIN, []);
        }

        var updatedPlugin = await _pluginRepository.StorePlugin(plugin);
        await _unitOfWork.CompleteAsync();

        return updatedPlugin;
    }

    private async Task AddUpdatedPluginNameLog(Plugin plugin, PatchGlobalPluginCommand request)
    {
        if (request.PluginName != null)
        {
            var changes = new List<LogChange>
            {
                new()
                {
                    Property = nameof(plugin.PluginName),
                    OldValue = plugin.PluginName,
                    NewValue = request.PluginName
                }
            };

            await _logRepository.AddGlobalPluginLogForCurrentUser(plugin, Action.UPDATED_GLOBAL_PLUGIN, changes);
        }
    }
}
