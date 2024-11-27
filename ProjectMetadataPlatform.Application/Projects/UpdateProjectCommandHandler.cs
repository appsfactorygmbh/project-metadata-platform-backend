using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateProjectCommand"/>.
    /// </summary>
    public UpdateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository, ILogRepository logRepository, IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// Handles the request to update a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    public async Task<int> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectWithPluginsAsync(request.Id)
                      ?? throw new InvalidOperationException("Project does not exist.");

        await UpdateProjectProperties(request, project);

        var invalidPluginIds = await request.Plugins
            .Select(plugin => plugin.PluginId)
            .Distinct()
            .ToAsyncEnumerable()
            .WhereAwait(async pluginId => !await _pluginRepository.CheckPluginExists(pluginId))
            .ToListAsync(cancellationToken);

        if (invalidPluginIds.Count > 0)
        {
            throw new InvalidOperationException(
                "The Plugins with these ids do not exist: " + string.Join(", ", invalidPluginIds));
        }

        var currentPlugins = new List<ProjectPlugins>(project.ProjectPlugins ?? []);

        var existingPlugins = currentPlugins
            .IntersectBy(request.Plugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var newPlugins = request.Plugins
            .ExceptBy(currentPlugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var removedPlugins = currentPlugins.Except(existingPlugins).ToList();

        var addedPluginChanges = new List<LogChange>();
        var removedPluginChanges = new List<LogChange>();
        var updatedPluginChanges = new List<LogChange>();

        foreach (var newPlugin in newPlugins)
        {
            addedPluginChanges.AddRange(
            [
                new LogChange
                {
                    Property = nameof(ProjectPlugins.PluginId),
                    OldValue = string.Empty,
                    NewValue = newPlugin.PluginId.ToString()
                },
                new LogChange
                {
                    Property = nameof(ProjectPlugins.ProjectId),
                    OldValue = string.Empty,
                    NewValue = newPlugin.ProjectId.ToString()
                },
                new LogChange
                {
                    Property = nameof(ProjectPlugins.DisplayName),
                    OldValue = string.Empty,
                    NewValue = newPlugin.DisplayName ?? string.Empty
                },
                new LogChange { Property =nameof(ProjectPlugins.Url), OldValue = string.Empty, NewValue = newPlugin.Url }
            ]);
        }

        foreach (var removedPlugin in removedPlugins)
        {
            removedPluginChanges.AddRange(
            [
                new LogChange
                {
                    Property = nameof(ProjectPlugins.PluginId),
                    OldValue = removedPlugin.PluginId.ToString(),
                    NewValue = string.Empty
                },
                new LogChange
                {
                    Property = nameof(ProjectPlugins.ProjectId),
                    OldValue = removedPlugin.ProjectId.ToString(),
                    NewValue = string.Empty
                },
                new LogChange
                {
                    Property = nameof(ProjectPlugins.DisplayName),
                    OldValue = removedPlugin.DisplayName ?? string.Empty,
                    NewValue = string.Empty
                },
                new LogChange
                {
                    Property = nameof(ProjectPlugins.Url), OldValue = removedPlugin.Url, NewValue = string.Empty
                }
            ]);
        }

        foreach (var existingPlugin in existingPlugins)
        {
            var requestPlugin = request.Plugins.First(plugin
                => GetProjectPluginKey(plugin) == GetProjectPluginKey(existingPlugin));

            if (existingPlugin.DisplayName != requestPlugin.DisplayName)
            {
                updatedPluginChanges.Add(new LogChange
                {
                    Property = nameof(ProjectPlugins.DisplayName),
                    OldValue = existingPlugin.DisplayName ?? string.Empty,
                    NewValue = requestPlugin.DisplayName ?? string.Empty
                });
                existingPlugin.DisplayName = requestPlugin.DisplayName;
            }

            if (existingPlugin.Url == requestPlugin.Url)
            {
                continue;
            }

            updatedPluginChanges.Add(new LogChange
            {
                Property = nameof(ProjectPlugins.Url),
                OldValue = existingPlugin.Url,
                NewValue = requestPlugin.Url
            });
            existingPlugin.Url = requestPlugin.Url;
        }

        project.ProjectPlugins = (project.ProjectPlugins ?? Enumerable.Empty<ProjectPlugins>())
            .Except(removedPlugins ?? Enumerable.Empty<ProjectPlugins>())
            .Concat(newPlugins ?? Enumerable.Empty<ProjectPlugins>())
            .ToList();

        if (addedPluginChanges.Count > 0)
        {
            await _logRepository.AddLogForCurrentUser(project.Id, Action.ADDED_PROJECT_PLUGIN, addedPluginChanges);
        }

        if (removedPluginChanges.Count > 0)
        {
            await _logRepository.AddLogForCurrentUser(project.Id, Action.REMOVED_PROJECT_PLUGIN, removedPluginChanges);
        }

        if (updatedPluginChanges.Count > 0)
        {
            await _logRepository.AddLogForCurrentUser(project.Id, Action.UPDATED_PROJECT_PLUGIN, updatedPluginChanges);
        }

        await _unitOfWork.CompleteAsync();

        return project.Id;
    }

    private static (int ProjectId, int PluginId, string Url) GetProjectPluginKey(ProjectPlugins projectPlugin)
        => (projectPlugin.ProjectId, projectPlugin.PluginId, projectPlugin.Url);

    private async Task UpdateProjectProperties(UpdateProjectCommand request, Project project)
    {
        var changes = new List<LogChange> {};

        if (project.ProjectName != request.ProjectName)
        {
            var change = new LogChange
            {
                Property = nameof(Project.ProjectName),
                OldValue = project.ProjectName,
                NewValue = request.ProjectName
            };
            changes.Add(change);
            project.ProjectName = request.ProjectName;
        }

        if (project.BusinessUnit != request.BusinessUnit)
        {
            var change = new LogChange
            {
                Property = nameof(Project.BusinessUnit),
                OldValue = project.BusinessUnit,
                NewValue = request.BusinessUnit
            };
            changes.Add(change);
            project.BusinessUnit = request.BusinessUnit;
        }

        if (project.TeamNumber != request.TeamNumber)
        {
            var change = new LogChange
            {
                Property = nameof(Project.TeamNumber),
                OldValue = project.TeamNumber.ToString(CultureInfo.InvariantCulture),
                NewValue = request.TeamNumber.ToString(CultureInfo.InvariantCulture)
            };
            changes.Add(change);
            project.TeamNumber = request.TeamNumber;
        }

        if (project.Department != request.Department)
        {
            var change = new LogChange
            {
                Property = nameof(Project.Department),
                OldValue = project.Department,
                NewValue = request.Department
            };
            changes.Add(change);
            project.Department = request.Department;
        }

        if (project.ClientName != request.ClientName)
        {
            var change = new LogChange
            {
                Property = nameof(Project.ClientName),
                OldValue = project.ClientName,
                NewValue = request.ClientName
            };
            changes.Add(change);
            project.ClientName = request.ClientName;
        }

        if (project.IsArchived != request.IsArchived)
        {
            var archivedChanges = new List<LogChange> {};

            var change = new LogChange()
            {
                Property = nameof(Project.IsArchived),
                OldValue = project.IsArchived.ToString(),
                NewValue = request.IsArchived.ToString()
            };
            archivedChanges.Add(change);

            await _logRepository.AddProjectLogForCurrentUser(project, request.IsArchived ? Action.ARCHIVED_PROJECT : Action.UNARCHIVED_PROJECT, archivedChanges);
            project.IsArchived = request.IsArchived;
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddProjectLogForCurrentUser(project, Action.UPDATED_PROJECT, changes);
        }
    }
}
