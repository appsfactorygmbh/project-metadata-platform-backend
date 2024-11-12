using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Handler for the <see cref="CreateProjectCommand" />.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    /// <summary>
    ///     Creates a new instance of <see cref="CreateProjectCommandHandler" />.
    /// </summary>
    /// <param name="projectsRepository"></param>
    /// <param name="pluginRepository"></param>
    public CreateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
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
        return project.Id;
    }
}
