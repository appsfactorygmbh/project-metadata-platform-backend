using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, IEnumerable<Team>>
{
    private readonly ITeamRepository _teamRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllTeamsQueryHandler" />.
    /// </summary>
    public GetAllTeamsQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Team>> Handle(
        GetAllTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _teamRepository.GetTeamsAsync(
            fullTextQuery: request.FullTextQuery,
            teamName: request.TeamName
        );
    }
}
