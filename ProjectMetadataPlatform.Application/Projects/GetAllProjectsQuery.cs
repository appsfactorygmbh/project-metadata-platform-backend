using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;


namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Query to get all projects or all projects with specific search pattern
///     <param name="Search">Search pattern to look for in ProjectName</param>
/// </summary>
public record GetAllProjectsQuery(ProjectFilterRequest? request, string? Search) : IRequest<IEnumerable<Project>>;
