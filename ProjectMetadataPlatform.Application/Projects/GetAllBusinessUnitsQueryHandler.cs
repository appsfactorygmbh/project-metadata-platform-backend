using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the retrieval of all business units within the system.
/// </summary>
/// <remarks>
/// This handler is part of the CQRS pattern implementation using MediatR. It processes
/// <see cref="GetAllBusinessUnitsQuery"/> to fetch a list of all business units.
/// Utilizes the <see cref="IProjectsRepository"/> for data access.
/// </remarks>
public class GetAllBusinessUnitsQueryHandler
    : IRequestHandler<GetAllBusinessUnitsQuery, IEnumerable<string>>
{
    private readonly IProjectsRepository _projectsRepository; // Repository interface for accessing projects data.

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllBusinessUnitsQueryHandler"/> class.
    /// </summary>
    /// <param name="projectsRepository">The projects repository.</param>
    public GetAllBusinessUnitsQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the <see cref="GetAllBusinessUnitsQuery"/> request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all business units.</returns>
    public Task<IEnumerable<string>> Handle(
        GetAllBusinessUnitsQuery request,
        CancellationToken cancellationToken
    )
    {
        return _projectsRepository.GetBusinessUnitsAsync();
    }
}
