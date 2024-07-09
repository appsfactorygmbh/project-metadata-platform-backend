using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the query to retrieve projects filtered by business unit and/or team number.
/// </summary>

public class GetBusinessUnitAndTeamNumberQueryHandler : IRequestHandler<GetBusinessUnitAndTeamNumberQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository; // Repository interface for accessing projects data.

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBusinessUnitAndTeamNumberQueryHandler"/> class.
    /// </summary>
    /// <param name="projectsRepository">The projects repository.</param>
    public GetBusinessUnitAndTeamNumberQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the <see cref="GetBusinessUnitAndTeamNumberQuery"/> request.
    /// </summary>
    /// <param name="request">The query request containing filter criteria.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of projects that match the specified filters.</returns>
    public Task<IEnumerable<Project>> Handle(GetBusinessUnitAndTeamNumberQuery request, CancellationToken cancellationToken)
    {
        if (request.BusinessUnit == null && request.TeamNumber == null)
        {
            return _projectRepository.GetProjectsAsync();
        }
        return _projectRepository.GetBusinessUnitAndTeamNumberAsync(request.BusinessUnit, request.TeamNumber);
    }

}
