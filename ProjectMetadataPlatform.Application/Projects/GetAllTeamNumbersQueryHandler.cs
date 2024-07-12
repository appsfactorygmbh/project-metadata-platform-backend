using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the retrieval of all distinct team numbers from the projects.
/// </summary>
public class GetAllTeamNumbersQueryHandler: IRequestHandler<GetAllTeamNumbersQuery, IEnumerable<int>>
{
    private readonly IProjectsRepository _projectsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllTeamNumbersQueryHandler"/> class.
    /// </summary>
    /// <param name="projectsRepository">The projects repository to access project data.</param>
    public GetAllTeamNumbersQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the request to get all distinct team numbers.
    /// </summary>
    /// <param name="request">The request object containing any necessary data.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of all distinct team numbers.</returns>
    public Task<IEnumerable<int>> Handle(GetAllTeamNumbersQuery request, CancellationToken cancellationToken)
    {
        return _projectsRepository.GetTeamNumbersAsync();
    }
}
