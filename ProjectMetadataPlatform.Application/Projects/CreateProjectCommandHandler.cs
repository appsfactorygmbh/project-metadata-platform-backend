using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handler for the <see cref="CreateProjectCommand" />.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugHelper _slugHelper;

    /// <summary>
    /// Creates a new instance of <see cref="CreateProjectCommandHandler" />.
    /// </summary>
    /// <param name="projectsRepository">Repository for Projects</param>
    /// <param name="pluginRepository">Repository for Plugins</param>
    /// <param name="logRepository">Repository for Logs</param>
    /// <param name="unitOfWork"> Used to save changes to the DbContext</param>
    /// <param name="slugHelper"> Used to generate slugs</param>
    public CreateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository,
        ILogRepository logRepository, IUnitOfWork unitOfWork, ISlugHelper slugHelper)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _slugHelper = slugHelper;
    }

    /// <summary>
    /// Handles the request to create a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        foreach (var plugin in request.Plugins)
        {
            if (!await _pluginRepository.CheckPluginExists(plugin.PluginId))
            {
                throw new InvalidOperationException("The Plugin with this id does not exist: " + plugin.PluginId);
            }
        }

        var projectSlug = _slugHelper.GenerateSlug(request.ProjectName);

        if (await _slugHelper.CheckProjectSlugExists(projectSlug))
        {
            throw new InvalidOperationException("A Project with this slug already exists: " + projectSlug);
        }

        var project = new Project
        {
            ProjectName = request.ProjectName, Slug = projectSlug, BusinessUnit = request.BusinessUnit,
            TeamNumber = request.TeamNumber, Department = request.Department, ClientName = request.ClientName,
            OfferId = request.OfferId, Company = request.Company, CompanyState = request.CompanyState,
            IsmsLevel = request.IsmsLevel, ProjectPlugins = request.Plugins
        };

        await _projectsRepository.Add(project);

        var changes = new List<LogChange>
        {
            new() { OldValue = "", NewValue = project.ProjectName, Property = nameof(Project.ProjectName) },
            new() { OldValue = "", NewValue = project.Slug, Property = nameof(Project.Slug) },
            new() { OldValue = "", NewValue = project.BusinessUnit, Property = nameof(Project.BusinessUnit) },
            new() { OldValue = "", NewValue = project.Department, Property = nameof(Project.Department) },
            new() { OldValue = "", NewValue = project.ClientName, Property = nameof(Project.ClientName) },
            new() { OldValue = "", NewValue = project.TeamNumber.ToString(), Property = nameof(Project.TeamNumber) },
            new() { OldValue = "", NewValue = project.OfferId, Property = nameof(Project.OfferId) },
            new() { OldValue = "", NewValue = project.Company, Property = nameof(Project.Company) },
            new() { OldValue = "", NewValue = project.CompanyState.ToString(), Property = nameof(Project.CompanyState) },
            new() { OldValue = "", NewValue = project.IsmsLevel.ToString(), Property = nameof(Project.IsmsLevel) }
        };
        await _logRepository.AddProjectLogForCurrentUser(project, Domain.Logs.Action.ADDED_PROJECT, changes);
        foreach (var plugin in project.ProjectPlugins)
        {
            var pluginChanges = new List<LogChange>
            {
                new() { OldValue = "", NewValue = plugin.Url, Property = nameof(ProjectPlugins.Url) }
            };
            if (plugin.DisplayName != null)
            {
                pluginChanges.Add(new LogChange
                    { OldValue = "", NewValue = plugin.DisplayName, Property = nameof(ProjectPlugins.DisplayName) });
            }

            await _logRepository.AddProjectLogForCurrentUser(project, Domain.Logs.Action.ADDED_PROJECT_PLUGIN,
                pluginChanges);
        }

        await _unitOfWork.CompleteAsync();
        return project.Id;
    }
}