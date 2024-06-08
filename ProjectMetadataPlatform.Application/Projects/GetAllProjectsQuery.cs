using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// </summary>
public record GetAllProjectsQuery(string? Search): IRequest<IEnumerable<Project>>;