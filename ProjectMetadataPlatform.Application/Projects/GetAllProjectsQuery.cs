using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// <param name="Search">Search pattern to look for in ProjectName</param>
/// </summary>
public record GetAllProjectsQuery(string Search): IRequest<IEnumerable<Project>>;