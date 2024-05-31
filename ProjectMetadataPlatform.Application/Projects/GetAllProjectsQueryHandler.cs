using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///  <inheritdoc />
/// </summary>
public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;
    
    /// <summary>
    /// Creates a new instance of <see cref="GetAllProjectsQueryHandler"/>.
    /// </summary>
    public GetAllProjectsQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectRepository = projectsRepository;
    }

    /// <inheritdoc />
    public Task<IEnumerable<Project>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        return _projectRepository.GetAllProjectsAsync();
    }
}