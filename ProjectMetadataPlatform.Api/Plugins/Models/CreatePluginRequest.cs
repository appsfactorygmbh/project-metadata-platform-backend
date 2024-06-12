namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Request for creating a new plugin
/// </summary>
/// <param name="PluginName"></param>
public record CreatePluginRequest(string PluginName);
