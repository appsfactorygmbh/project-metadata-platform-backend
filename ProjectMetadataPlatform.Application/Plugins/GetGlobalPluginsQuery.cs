using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

public record GetGlobalPluginsQuery() : IRequest<List<Plugin>>;
