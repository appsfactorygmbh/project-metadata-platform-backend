using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the GetProjectsByBusinessUnitsQuery.
/// </summary>
public class GetProjectsByBusinessUnitsQueryHandler : IRequestHandler<GetProjectsByBusinessUnitsQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByBusinessUnitsQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The repository of projects.</param>
    public GetProjectsByBusinessUnitsQueryHandler(IProjectsRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the GetProjectsByBusinessUnitsQuery.
    /// </summary>
    /// <param name="request">The GetProjectsByBusinessUnitsQuery.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>The task result contains the projects that belong to the specified business units.</returns>
    public async Task<IEnumerable<Project>> Handle(GetProjectsByBusinessUnitsQuery request, CancellationToken cancellationToken)
    {
        return await _projectRepository.GetProjectsByBusinessUnitsAsync(request.BusinessUnits);
    }
}
