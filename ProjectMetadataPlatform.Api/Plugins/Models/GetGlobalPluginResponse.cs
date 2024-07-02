namespace ProjectMetadataPlatform.Api.Plugins.Models;
/// <summary>
/// Response Model representing a Plugin.
/// </summary>
/// <param name="Name">Name of the plugin</param>
/// <param name="Id">Id of the plugin</param>
/// <param name="Archived">If the plugin is archived or not</param>
/// <param name="Keys">empty array keys</param>
public record GetGlobalPluginResponse(string Name, int Id, bool Archived, string[] Keys);
