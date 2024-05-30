namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Project properties.
/// </summary>
/// <param name="projectName">The name of the project.</param>
/// <param name="clientName">The name of the client.</param>
/// <param name="businessUnit">The Business Units id.</param>
/// <param name="teamNumber">The team responsible for the project.</param>
// TODO Add list of plugins as property.
public record Project(string projectName, string clientName, string businessUnit, int teamNumber);
