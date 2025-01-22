using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

public class PluginNameAlreadyExistsException(string name) : EntityAlreadyExistsException("A global Plugin with the name " + name + " already exists.");