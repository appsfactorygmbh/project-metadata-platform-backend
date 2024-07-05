using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Query to get a project by ClientName
/// </summary>
public record SearchProjectsClientNameQuery(string Search) : IRequest<IEnumerable<Project>?>;
