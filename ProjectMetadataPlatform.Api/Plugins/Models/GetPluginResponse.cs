namespace ProjectMetadataPlatform.Api.Plugins.Models;


/// <summary>
/// Response for getting a Plugin.
/// </summary>
/// <param name="PluginName"></param>
/// <param name="Url"></param>
/// <param name="DisplayName"></param>
public record GetPluginResponse(string PluginName, string Url, string DisplayName);
