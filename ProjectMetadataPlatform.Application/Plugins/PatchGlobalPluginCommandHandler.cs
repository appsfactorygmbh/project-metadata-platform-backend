using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handles the PatchGlobalPluginCommand request.
/// </summary>
public class PatchGlobalPluginCommandHandler : IRequestHandler<PatchGlobalPluginCommand, Plugin?>
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
    public PatchGlobalPluginCommandHandler(IPluginRepository pluginRepository, ILogRepository logRepository, IUnitOfWork unitOfWork)
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
    public async Task<Plugin?> Handle(PatchGlobalPluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id) ?? throw new InvalidOperationException("Plugin not found.");

        var changes = new List<LogChange>();

        var archivedStateChanges = new List<LogChange>();

        if (request.PluginName != null && !string.Equals(plugin.PluginName, request.PluginName, StringComparison.Ordinal))
        {
            changes.Add(new LogChange
            {
                Property = nameof(plugin.PluginName),
                OldValue = plugin.PluginName,
                NewValue = request.PluginName
            });

            plugin.PluginName = request.PluginName;

            await _logRepository.AddLogForCurrentUser(plugin, Action.UPDATED_GLOBAL_PLUGIN, changes);

        }

        if (request.IsArchived != null && plugin.IsArchived != request.IsArchived.Value)
        {
            archivedStateChanges.Add(new LogChange
            {
                Property = nameof(plugin.IsArchived),
                OldValue = plugin.IsArchived.ToString(),
                NewValue = request.IsArchived.Value.ToString()
            });

            plugin.IsArchived = request.IsArchived.Value;

            if (plugin.IsArchived)
            {
                await _logRepository.AddLogForCurrentUser(plugin, Action.ARCHIVED_GLOBAL_PLUGIN, archivedStateChanges);
            }
            else
            {
                await _logRepository.AddLogForCurrentUser(plugin, Action.UNARCHIVED_GLOBAL_PLUGIN, archivedStateChanges);
            }
        }

        Plugin updatedPlugin = await _pluginRepository.StorePlugin(plugin);
        await _unitOfWork.CompleteAsync();

        return updatedPlugin;
    }
}
