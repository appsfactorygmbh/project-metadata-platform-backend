using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a plugin is not found.
/// </summary>
public class PluginNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
    /// </summary>
    /// <param name="id">Id of the plugin that was searched for.</param>
    public PluginNotFoundException(int id): base("The plugin with the id " + id + " was not found.")
    {
    }
}