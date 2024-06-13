using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
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
    public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project{ProjectName=request.ProjectName, BusinessUnit=request.BusinessUnit, TeamNumber=request.TeamNumber, Department=request.Department, ClientName=request.ClientName};
        var storedProject = await _projectsRepository.Updatewithreturnvalue(project);
        return storedProject;
    }
}
