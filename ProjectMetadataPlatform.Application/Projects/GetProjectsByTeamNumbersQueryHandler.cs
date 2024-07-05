using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the GetProjectsByTeamNumbersQuery.
/// </summary>
public class GetProjectsByTeamNumbersQueryHandler : IRequestHandler<GetProjectsByTeamNumbersQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByTeamNumbersQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The repository of projects.</param>
    public GetProjectsByTeamNumbersQueryHandler(IProjectsRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Handles the GetProjectsByTeamNumbersQuery.
    /// </summary>
    /// <param name="request">The GetProjectsByTeamNumbersQuery.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>The task result contains the projects that belong to the specified team numbers.</returns>
    public async Task<IEnumerable<Project>> Handle(GetProjectsByTeamNumbersQuery request, CancellationToken cancellationToken)
    {
        return await _projectRepository.GetProjectsByTeamNumbersAsync(request.TeamNumbers);
    }
}
