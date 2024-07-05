using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using System.Linq;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class SearchProjectsClientNameQueryHandler : IRequestHandler<SearchProjectsClientNameQuery, IEnumerable<Project>?>
{
    private readonly IProjectsRepository _projectRepository;

    public SearchProjectsClientNameQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectRepository = projectsRepository;
    }

    public async Task<IEnumerable<Project>?> Handle(SearchProjectsClientNameQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Search))
        {
            // Return an empty IEnumerable<Project> if search string is null or whitespace
            return Enumerable.Empty<Project>();
        }
        else
        {
            // Get projects matching the search criteria
            var projects = await _projectRepository.GetProjectsClientNameAsync(request.Search);
            return projects ?? Enumerable.Empty<Project>();
        }
    }
}
