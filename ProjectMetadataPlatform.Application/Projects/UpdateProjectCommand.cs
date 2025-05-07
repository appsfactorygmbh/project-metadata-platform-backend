using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Command to update a project with the given attributes.
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
/// <param name="Id">Id of the project</param>
/// <param name="Plugins">List of plugins associated with the project</param>
/// <param name="IsArchived">Indicates if the project is archived</param>
public record UpdateProjectCommand(
    string ProjectName,
    string BusinessUnit,
    int TeamNumber,
    string Department,
    string ClientName,
    string OfferId,
    string Company,
    CompanyState CompanyState,
    SecurityLevel IsmsLevel,
    int Id,
    List<ProjectPlugins> Plugins,
    bool IsArchived
) : IRequest<int>;
