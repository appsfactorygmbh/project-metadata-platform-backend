using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    /// <summary>
    /// Creates a new instance of <see cref="UpdateProjectCommand"/>.
    /// </summary>
    /// <param name="projectsRepository"></param>
    public UpdateProjectCommandHandler(IProjectsRepository projectsRepository, IPluginRepository pluginRepository)
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
    }
    /// <summary>
    /// Handles the request to update a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    public async Task<int> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project{ProjectName=request.ProjectName, BusinessUnit=request.BusinessUnit, TeamNumber=request.TeamNumber, Department=request.Department, ClientName=request.ClientName, Id = request.Id, ProjectPlugins = request.Plugins};

        if (await _projectsRepository.CheckProjectExists(project.Id))
        {
            foreach (var plugin in project.ProjectPlugins)
            {
                if (!(await _pluginRepository.CheckPluginExists(plugin.PluginId)))
                {
                   throw new InvalidOperationException("The Plugin with this id does not exist: " + plugin.PluginId);
                }
            }
            await _projectsRepository.UpdateProject(project,request.Plugins);
        }
        else
        {
            throw new InvalidOperationException("Project does not exist.");
        }
        return project.Id;
    }
}
