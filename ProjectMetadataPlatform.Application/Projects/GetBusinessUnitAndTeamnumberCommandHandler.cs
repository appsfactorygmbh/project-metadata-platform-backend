using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public class GetBusinessUnitAndTeamnumberCommandHandler : IRequestHandler<GetBusinessUnitAndTeamnumberQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;
    
    public GetBusinessUnitAndTeamnumberCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectRepository = projectsRepository;
    }

    public Task<IEnumerable<Project>> Handle(GetBusinessUnitAndTeamnumberQuery request, CancellationToken cancellationToken)
    {
        return _projectRepository.GetBusinessUnitAndTeamnumberAsync(request.BusinessUnit, request.TeamNumber);
    }
    
}
