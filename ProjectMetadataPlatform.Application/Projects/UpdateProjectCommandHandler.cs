using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateProjectCommand"/>.
    /// </summary>
    public UpdateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository, IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
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

        UpdateProjectProperties(request, project);

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

        var currentPlugins = new List<ProjectPlugins>(project.ProjectPlugins!);

        var existingPlugins = currentPlugins
            .IntersectBy(request.Plugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var newPlugins = request.Plugins
            .ExceptBy(currentPlugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var removedPlugins = currentPlugins.Except(existingPlugins).ToList();

        foreach (var existingPlugin in existingPlugins)
        {
            var requestPlugin = request.Plugins.First(plugin
                => GetProjectPluginKey(plugin) == GetProjectPluginKey(existingPlugin));

            if (existingPlugin.DisplayName != requestPlugin.DisplayName)
            {
                existingPlugin.DisplayName = requestPlugin.DisplayName;
            }
        }

        project.ProjectPlugins = project.ProjectPlugins!
            .Except(removedPlugins)
            .Concat(newPlugins)
            .ToList();

        await _unitOfWork.CompleteAsync();

        return project.Id;
    }

    private static (int ProjectId, int PluginId, string Url) GetProjectPluginKey(ProjectPlugins projectPlugin)
        => (projectPlugin.ProjectId, projectPlugin.PluginId, projectPlugin.Url);

    private static void UpdateProjectProperties(UpdateProjectCommand request, Project project)
    {
        var changedProperties = new List<(string Name, string Old, string New)>();

        if (project.ProjectName != request.ProjectName)
        {
            changedProperties.Add((nameof(Project.ProjectName), project.ProjectName, request.ProjectName));
            project.ProjectName = request.ProjectName;
        }

        if (project.BusinessUnit != request.BusinessUnit)
        {
            changedProperties.Add((nameof(Project.BusinessUnit), project.BusinessUnit, request.BusinessUnit));
            project.BusinessUnit = request.BusinessUnit;
        }

        if (project.TeamNumber != request.TeamNumber)
        {
            changedProperties.Add((nameof(Project.TeamNumber), project.TeamNumber.ToString(CultureInfo.InvariantCulture), request.TeamNumber.ToString(CultureInfo.InvariantCulture)));
            project.TeamNumber = request.TeamNumber;
        }

        if (project.Department != request.Department)
        {
            changedProperties.Add((nameof(Project.Department), project.Department, request.Department));
            project.Department = request.Department;
        }

        if (project.ClientName != request.ClientName)
        {
            changedProperties.Add((nameof(Project.ClientName), project.ClientName, request.ClientName));
            project.ClientName = request.ClientName;
        }
        if (request.isArchived.HasValue)
        {
            if (project.IsArchived != request.isArchived.Value)
            {
                changedProperties.Add((nameof(Project.IsArchived), project.IsArchived.ToString(), request.isArchived.Value.ToString()));
                project.IsArchived = request.isArchived.Value;
            }
        }
    }
}
