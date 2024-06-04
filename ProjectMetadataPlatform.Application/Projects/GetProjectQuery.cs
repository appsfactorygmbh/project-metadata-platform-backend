using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetProjectQuery(int Id): IRequest<Project>;