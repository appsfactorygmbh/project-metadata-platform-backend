namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
///     Represents a response to the GetProject API call.
/// </summary>
/// <param name="Id">The identification number for the project.</param>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client for the project.</param>
/// <param name="BusinessUnit">The name of the Business Unit associated with the project.</param>
/// <param name="TeamNumber">The number of the team working on the project.</param>
/// <param name="Department">The name of the department associated with the project.</param>
/// <param name="IsArchived">If the project is archived or not.</param>
public record GetProjectResponse(
    int Id,
    string ProjectName,
    string ClientName,
    string BusinessUnit,
    int TeamNumber,
    string Department,
    bool IsArchived);
