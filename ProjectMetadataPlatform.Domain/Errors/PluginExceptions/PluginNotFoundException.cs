using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

public class PluginNotFoundException : EntityNotFoundException
{
    public PluginNotFoundException(int Id): base("The plugin with the id " + Id + " was not found.")
    {
    }
}