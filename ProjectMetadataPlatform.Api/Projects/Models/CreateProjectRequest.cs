namespace ProjectMetadataPlatform.Api.Projects.Models;
/// <summary>
/// Represents the request for the CreateProject API call.
/// </summary>

/// <param name="ProjectName">Name of the project</param>
/// <param name="ClientName">Name of the client for the project</param>
/// <param name="BusinessUnit">Name of the Business Unit associated with the project</param>
/// <param name="TeamNumber">Number of the team working on the project</param>
/// <param name="Department">Name of the department associated with the project</param>
public record CreateProjectRequest(string ProjectName, string BusinessUnit, int TeamNumber, string Department, string ClientName);
