using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
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
    /// <exception cref="ProjectSlugAlreadyExistsException">When a project with the same slug already exists.</exception>
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        foreach (var plugin in request.Plugins)
        {
            if (!await _pluginRepository.CheckPluginExists(plugin.PluginId))
            {
                throw new PluginNotFoundException(plugin.PluginId);
            }
        }

        var projectSlug = _slugHelper.GenerateSlug(request.ProjectName);

        if (await _slugHelper.CheckProjectSlugExists(projectSlug))
        {
            throw new ProjectSlugAlreadyExistsException(projectSlug);
        }

        var project = new Project
        {
            ProjectName = request.ProjectName, Slug = projectSlug, BusinessUnit = request.BusinessUnit,
            TeamNumber = request.TeamNumber, Department = request.Department, ClientName = request.ClientName,
            OfferId = request.OfferId, Company = request.Company, CompanyState = request.CompanyState,
            IsmsLevel = request.IsmsLevel, ProjectPlugins = request.Plugins
        };

        await _projectsRepository.Add(project);

        await AddCreatedProjectLog(project);

        await _unitOfWork.CompleteAsync();
        return project.Id;
    }

    private async Task AddCreatedProjectLog(Project project)
    {
        var changes = new List<LogChange>
        {
            new() { OldValue = "", NewValue = project.ProjectName, Property = nameof(Project.ProjectName) },
            new() { OldValue = "", NewValue = project.Slug, Property = nameof(Project.Slug) },
            new() { OldValue = "", NewValue = project.BusinessUnit, Property = nameof(Project.BusinessUnit) },
            new() { OldValue = "", NewValue = project.Department, Property = nameof(Project.Department) },
            new() { OldValue = "", NewValue = project.ClientName, Property = nameof(Project.ClientName) },
            new() { OldValue = "", NewValue = project.TeamNumber.ToString(CultureInfo.InvariantCulture), Property = nameof(Project.TeamNumber) },
            new() { OldValue = "", NewValue = project.OfferId, Property = nameof(Project.OfferId) },
            new() { OldValue = "", NewValue = project.Company, Property = nameof(Project.Company) },
            new() { OldValue = "", NewValue = project.CompanyState.ToString(), Property = nameof(Project.CompanyState) },
            new() { OldValue = "", NewValue = project.IsmsLevel.ToString(), Property = nameof(Project.IsmsLevel) }
        };
        await _logRepository.AddProjectLogForCurrentUser(project, Action.ADDED_PROJECT, changes);
        if (project.ProjectPlugins != null)
        {
            foreach (var plugin in project.ProjectPlugins)
            {
                var pluginChanges = new List<LogChange>
                {
                    new() { OldValue = "", NewValue = plugin.Url, Property = nameof(ProjectPlugins.Url) }
                };
                if (plugin.DisplayName != null)
                {
                    pluginChanges.Add(new LogChange
                    {
                        OldValue = "", NewValue = plugin.DisplayName, Property = nameof(ProjectPlugins.DisplayName)
                    });
                }

                await _logRepository.AddProjectLogForCurrentUser(project, Action.ADDED_PROJECT_PLUGIN,
                    pluginChanges);
            }
        }
    }
}