using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

public class GetAllBusinessUnitsQueryHandler: IRequestHandler<GetAllBusinessUnitsQuery, IEnumerable<string>>
{
    private readonly IProjectsRepository _projectsRepository;

    public GetAllBusinessUnitsQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;

    }

    public Task<IEnumerable<string>> Handle(GetAllBusinessUnitsQuery request, CancellationToken cancellationToken)
    {
        return _projectsRepository.GetBusinessUnitsAsync();
    }
}
