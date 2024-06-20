namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Request for creating a new plugin.
/// </summary>
/// <param name="PluginName">The name of the new plugin.</param>
public record CreatePluginRequest(string PluginName);
