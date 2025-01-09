using System.Collections.Generic;

namespace ProjectMetadataPlatform.Application.Projects;


/// <summary>
/// Represents a request to filter projects based on various criteria.
/// </summary>
/// <param name="ProjectName">Optional. The name of the project to filter by.</param>
/// <param name="ClientName">Optional. The name of the client associated with the project to filter by.</param>
/// <param name="BusinessUnit">Optional. A list of business units to filter the projects by.</param>
/// <param name="TeamNumber">Optional. A list of team numbers to filter the projects by.</param>
/// <param name="IsArchived">Optional. The archival status of the projects to filter by.</param>
public record ProjectFilterRequest(string? ProjectName, string? ClientName, List<string>? BusinessUnit, List<int>? TeamNumber, bool? IsArchived, List<string>? Company, string? IsmsLevel );
