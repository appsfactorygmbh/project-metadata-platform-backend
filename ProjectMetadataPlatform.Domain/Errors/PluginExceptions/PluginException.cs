namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

public abstract class PluginException(string message) : PmpException(message);
