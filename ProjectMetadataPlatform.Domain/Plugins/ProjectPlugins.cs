using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Plugins;

public class ProjectPlugins
{
    public Project? Project { get; set; }

    public Plugin? Plugin { get; set; }

    public int PluginId { get; set; }

    public int ProjectId { get; set; }

    public string DisplayName { get; set; }

    public string Url { get; set; }
}
