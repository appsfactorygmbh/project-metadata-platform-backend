using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Command to create a new project with the given attributes.
/// </summary>
/// <param name="ProjectName">Name of the project</param>
/// <param name="BusinessUnit">Name of the business unit</param>
/// <param name="TeamNumber">Number of the team</param>
/// <param name="Department">Name of the department</param>
/// <param name="ClientName">Name of the client</param>
public record CreateProjectCommand(
    string ProjectName,
    string BusinessUnit,
    int TeamNumber,
    string Department,
    string ClientName,
    List<ProjectPlugins> Plugins) : IRequest<int>;
