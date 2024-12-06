using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetProjectIdBySlugQuery(string slug) : IRequest<int?>;