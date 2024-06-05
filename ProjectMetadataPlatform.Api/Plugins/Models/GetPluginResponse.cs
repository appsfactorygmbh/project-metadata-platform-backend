namespace ProjectMetadataPlatform.Api.Plugins.Models;


/// <summary>
/// Response for getting a plugin.
/// </summary>
/// <param name="PluginName">Name of the plugin.</param>
/// <param name="Url">Url of the plugin.</param>
public record GetPluginResponse(string PluginName, string Url);
