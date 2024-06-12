using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Command to create a new Plugin with the given name.
/// </summary>
/// <param name="name">The name of the new plugin</param>
public record CreatePluginCommand(string name): IRequest<Plugin>;
