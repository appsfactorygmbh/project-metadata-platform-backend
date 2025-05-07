using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a response for the GetProjects API call.
/// </summary>
/// <param name="Id">The id of the project.</param>
/// <param name="Slug">The Slug for the project name.</param>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client associated with the project.</param>
/// <param name="BusinessUnit">The business unit the project belongs to.</param>
/// <param name="TeamNumber">The number of the team working on the project.</param>
/// <param name="IsArchived">If the project is archived or not.</param>
/// <param name="Company">The company associated with the project.</param>
/// <param name="IsmsLevel">The ISMS (Information Security Management System) level of the project.</param>
public record GetProjectsResponse(
    int Id,
    string Slug,
    string ProjectName,
    string ClientName,
    string BusinessUnit,
    int TeamNumber,
    bool IsArchived,
    string Company,
    SecurityLevel IsmsLevel
);
