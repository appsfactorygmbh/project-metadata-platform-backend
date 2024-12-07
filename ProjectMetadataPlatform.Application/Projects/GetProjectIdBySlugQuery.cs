using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///    Query to get a project by slug.
/// </summary>
/// <param name="Slug">Slug of the project.</param>
public record GetProjectIdBySlugQuery(string Slug) : IRequest<int?>;