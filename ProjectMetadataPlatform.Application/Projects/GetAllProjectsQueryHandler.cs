using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;


///  <inheritdoc />
public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;
    private IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>> _requestHandlerImplementation;

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
        try
        {
            return request.search == null
                ? _projectRepository.GetProjectsAsync()
                : _projectRepository.GetProjectsAsync(request.search);
        }
        catch
        {
            
        }

        return _projectRepository.GetProjectsAsync();

    }
    
    
}
