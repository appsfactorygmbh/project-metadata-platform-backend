namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents the response for the GetProject API call.
/// </summary>
/// <param name="Id">Identification number for the project</param>
/// <param name="ProjectName">Name of the project</param>
/// <param name="ClientName">Name of the client for the project</param>
/// <param name="BusinessUnit">Name of the Business Unit associated with the project</param>
/// <param name="TeamNumber">Number of the team working on the project</param>
/// <param name="Department">Name of the department associated with the project</param>
public record GetProjectResponse(int Id, string ProjectName, string ClientName, string BusinessUnit, int TeamNumber, string Department)
{
    
}