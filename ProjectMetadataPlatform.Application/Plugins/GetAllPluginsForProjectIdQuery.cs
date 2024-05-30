using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get weather forecasts.
/// </summary>
/// <param name="Count">How many weather forecasts to retrieve.</param>
public record GetAllPluginsForProjectIdQuery(int Id) : IRequest<IEnumerable<Plugin>>;