namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Project properties.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client.</param>
/// <param name="BusinessUnit">The Business Units id.</param>
/// <param name="TeamNumber">The team responsible for the project.</param>
// TODO Add list of plugins as property.
public record Project(string ProjectName, string ClientName, string BusinessUnit, int TeamNumber);
