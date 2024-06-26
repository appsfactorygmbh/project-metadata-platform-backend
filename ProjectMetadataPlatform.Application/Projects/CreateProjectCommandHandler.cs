using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    /// <summary>
    /// Creates a new instance of <see cref="CreateProjectCommandHandler"/>.
    /// </summary>
    /// <param name="projectsRepository"></param>
    public CreateProjectCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }
    /// <summary>
    /// Handles the request to create a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        Project project;
        if (request.ProjectId == null)
        {
            project = new Project{ProjectName=request.ProjectName, BusinessUnit=request.BusinessUnit, TeamNumber=request.TeamNumber, Department=request.Department, ClientName=request.ClientName};
        }
        else if (await _projectsRepository.CheckProjectExists(request.ProjectId.Value))
        {
            project = new Project{Id = request.ProjectId.Value, ProjectName=request.ProjectName, BusinessUnit=request.BusinessUnit, TeamNumber=request.TeamNumber, Department=request.Department, ClientName=request.ClientName};
        }
        else
        {
            throw new InvalidOperationException("Project does not exist.");
        }

        await _projectsRepository.AddOrUpdate(project);
        return project.Id;
    }
}
