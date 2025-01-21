using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

public class PluginNotArchivedException(Plugin plugin) : PluginException("The plugin " + plugin.Id + " is not archived.");
