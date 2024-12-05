using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the query to get a project ID by its slug.
/// </summary>
public class GetProjectIdBySlugQueryHandler : IRequestHandler<GetProjectIdBySlugQuery, int>
{
    private readonly ISlugHelper _slugHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectIdBySlugQueryHandler"/> class.
    /// </summary>
    /// <param name="slugHelper">The helper to manage slugs.</param>
    public GetProjectIdBySlugQueryHandler(ISlugHelper slugHelper)
    {
        _slugHelper = slugHelper;
    }

    /// <summary>
    /// Handles the request to get a project ID by its slug.
    /// </summary>
    /// <param name="request">The request containing the slug.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the project ID.</returns>
    public Task<int> Handle(GetProjectIdBySlugQuery request, CancellationToken cancellationToken)
    {
        return _slugHelper.GetProjectIdBySlug(request.Slug);
    }
}