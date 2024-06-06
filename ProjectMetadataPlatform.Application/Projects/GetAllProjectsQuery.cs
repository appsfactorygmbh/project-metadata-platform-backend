using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects.
/// </summary>
public record GetAllProjectsQuery(): IRequest<IEnumerable<Project>>;