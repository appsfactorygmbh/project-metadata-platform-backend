using System.Collections.Generic;

namespace ProjectMetadataPlatform.Application.Projects;

public class ProjectFilterRequest
{
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public List<string>? BusinessUnit { get; set; }
    public List<int>? TeamNumber { get; set; }
}
