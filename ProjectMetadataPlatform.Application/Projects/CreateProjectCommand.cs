using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
///     Command to create a new project with the given attributes.
/// </summary>
/// <param name="ProjectName">Name of the project</param>
/// <param name="BusinessUnit">Name of the business unit</param>
/// <param name="TeamNumber">Number of the team</param>
/// <param name="Department">Name of the department</param>
/// <param name="ClientName">Name of the client</param>
/// <param name="OfferId">Id of the offer associated with project.</param>
/// <param name="Company">Company responsible for project.</param>
/// <param name="CompanyState">State of company.</param>
/// <param name="IsmsLevel">Security Level of project.</param>
public record CreateProjectCommand(
    string ProjectName,
    string BusinessUnit,
    int TeamNumber,
    string Department,
    string ClientName,
    string OfferId,
    string Company,
    CompanyState CompanyState,
    SecurityLevel IsmsLevel,
    List<ProjectPlugins> Plugins) : IRequest<int>;
