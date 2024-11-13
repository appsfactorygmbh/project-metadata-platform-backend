using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Handler for the <see cref="CreateProjectCommand" />.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    ///     Creates a new instance of <see cref="CreateProjectCommandHandler" />.
    /// </summary>
    /// <param name="projectsRepository"></param>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    public CreateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository, ILogRepository logRepository, IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;

    }
    /// <summary>
    ///     Handles the request to create a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        foreach (var plugin in request.Plugins)
        {
            if (!(await _pluginRepository.CheckPluginExists(plugin.PluginId)))
            {
                throw new InvalidOperationException("The Plugin with this id does not exist: " + plugin.PluginId);
            }
        }
        var project = new Project{ProjectName=request.ProjectName, BusinessUnit=request.BusinessUnit, TeamNumber=request.TeamNumber, Department=request.Department, ClientName=request.ClientName, ProjectPlugins = request.Plugins};
        await _projectsRepository.Add(project);

        var changes = new List<LogChange>
        {
            new() { OldValue = "", NewValue = project.ProjectName, Property = "ProjectName" },
            new() { OldValue = "", NewValue = project.BusinessUnit, Property = "BusinessUnit" },
            new() { OldValue = "", NewValue = project.Department, Property = "Department" },
            new() { OldValue = "", NewValue = project.ClientName, Property = "ClientName" },
            new() { OldValue = "", NewValue = project.TeamNumber.ToString(), Property = "TeamNumber" }
        };
        await _logRepository.AddLogForCurrentUser(project.Id, Domain.Logs.Action.ADDED_PROJECT, changes);
        foreach (var plugin in project.ProjectPlugins)
        {
            var pluginChanges = new List<LogChange>
            {
                new() { OldValue = "", NewValue = plugin.Url, Property = "Url" }
            };
            if(plugin.DisplayName != null)
            {
                pluginChanges.Add(new LogChange { OldValue = "", NewValue = plugin.DisplayName, Property = "DisplayName" });
            }
            await _logRepository.AddLogForCurrentUser(project.Id, Domain.Logs.Action.ADDED_PROJECT_PLUGIN, pluginChanges);
        }
        await _unitOfWork.CompleteAsync();
        return project.Id;
    }
}
