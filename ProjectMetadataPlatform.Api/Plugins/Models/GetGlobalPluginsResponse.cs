namespace ProjectMetadataPlatform.Api.Plugins.Models;

public record GetGlobalPluginsResponse(string pluginName, int Id, bool isArchived, string[] keys);
