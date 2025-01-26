using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a request to create a new project.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client for the project.</param>
/// <param name="BusinessUnit">The name of the Business Unit associated with the project.</param>
/// <param name="TeamNumber">The number of the team working on the project.</param>
/// <param name="Department">The name of the department associated with the project.</param>
/// <param name="OfferId">Id of the offer associated with project.</param>
/// <param name="Company">Company responsible for project.</param>
/// <param name="CompanyState">State of company.</param>
/// <param name="IsmsLevel">Security Level of project.</param>
/// <param name="PluginList">An optional list of plugins associated to the project.</param>
/// <param name="IsArchived">Indicates if the project is archived.</param>
public record CreateProjectRequest(
    string ProjectName,
    string BusinessUnit,
    int TeamNumber,
    string Department,
    string ClientName,
    string OfferId,
    string Company,
    CompanyState CompanyState,
    SecurityLevel IsmsLevel,
    List<UpdateProjectPluginRequest>? PluginList = null,
    bool IsArchived = false);
