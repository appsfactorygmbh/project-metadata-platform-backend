using System.Collections.Generic;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Represents a request object used for filtering projects.
/// </summary>
public class ProjectFilterRequest
{
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public List<string>? BusinessUnit { get; set; }
    public List<int>? TeamNumber { get; set; }
}
