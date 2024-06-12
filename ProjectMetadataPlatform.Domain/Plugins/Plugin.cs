using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMetadataPlatform.Domain.Plugins;


/// <summary>
/// The representation of a plugin in the database.
/// </summary>
public class Plugin
{
    /// <summary>
    /// The id of the plugin.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public required string PluginName { get; set;}

    /// <summary>
    /// Holds the relation between Projects and Plugins.
    /// </summary>
    public ICollection<ProjectPlugins>? ProjectPlugins { get; set; }

}
