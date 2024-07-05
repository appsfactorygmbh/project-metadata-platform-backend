using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Query to get a project by ProjectName | ClientName
/// </summary>
public record SearchProjectsQuery(string Search) : IRequest<IEnumerable<Project>?>;
