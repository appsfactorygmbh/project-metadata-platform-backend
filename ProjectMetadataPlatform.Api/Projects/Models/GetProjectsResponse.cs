namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents the response for the GetProjects API call.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client associated with the project.</param>
/// <param name="BusinessUnit">The business unit the project belongs to.</param>
/// <param name="TeamNumber">The number of the team working on the project.</param>
public record GetProjectsResponse(string ProjectName, string ClientName, string BusinessUnit, int TeamNumber)
{
    
}