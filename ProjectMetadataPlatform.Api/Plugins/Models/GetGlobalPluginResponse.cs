namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Represents the response from getting a global plugin.
/// </summary>
/// <param name="Id">The unique identifier of the plugin.</param>
/// <param name="Name">The name of the plugin.</param>
/// <param name="IsArchived">The archived status of the plugin.</param>
/// <param name="Keys">An array of keys associated with the plugin.</param>
public record GetGlobalPluginResponse(int Id, string Name, bool IsArchived, string[] Keys);