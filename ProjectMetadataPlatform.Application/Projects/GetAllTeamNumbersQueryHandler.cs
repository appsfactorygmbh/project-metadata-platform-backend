using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

public class GetAllTeamNumbersQueryHandler: IRequestHandler<GetAllTeamNumbersQuery, IEnumerable<int>>
{

    private readonly IProjectsRepository _projectsRepository;

    public GetAllTeamNumbersQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public Task<IEnumerable<int>> Handle(GetAllTeamNumbersQuery request, CancellationToken cancellationToken)
    {

        return _projectsRepository.GetTeamNumbersAsync();
    }
}
