using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public class GetProjectsByBusinessUnitsQueryHandler : IRequestHandler<GetProjectsByBusinessUnitsQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;

    public GetProjectsByBusinessUnitsQueryHandler(IProjectsRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<Project>> Handle(GetProjectsByBusinessUnitsQuery request, CancellationToken cancellationToken)
    {
        /*
        if (request.BusinessUnits == null || request.BusinessUnits.Count == 0)
        {
            return Enumerable.Empty<Project>();
        }*/
        return await _projectRepository.GetProjectsByBusinessUnitsAsync(request.BusinessUnits);
    }
}
