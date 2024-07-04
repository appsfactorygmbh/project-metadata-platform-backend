namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Response for deleting a plugin.
/// </summary>
/// <param name="PluginId">The id of the plugin.</param>
/// <param name="IsArchived">The status whether the plugin is archived.</param>
public record DeleteGlobalPluginResponse(int PluginId, bool IsArchived);

