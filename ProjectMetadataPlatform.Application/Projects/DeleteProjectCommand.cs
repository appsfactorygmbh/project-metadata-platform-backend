using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public record DeleteProjectCommand(int Id) : IRequest<Project?>;
