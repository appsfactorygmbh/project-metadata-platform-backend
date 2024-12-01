namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Response for deleting a plugin.
/// </summary>
/// <param name="PluginId">The id of the plugin.</param>
/// <param name="Success">Wether the plugin was deleted or not</param>
public record DeleteGlobalPluginResponse(int PluginId, bool? Success);

