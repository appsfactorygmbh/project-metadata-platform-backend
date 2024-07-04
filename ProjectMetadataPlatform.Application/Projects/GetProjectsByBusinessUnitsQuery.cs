using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Represents a query to get projects by business units.
/// </summary>
/// <param name="BusinessUnits">A list of business units to filter the projects by.</param>
/// <returns>A request for an enumeration of projects that belong to the specified business units.</returns>
public record GetProjectsByBusinessUnitsQuery(List<string> BusinessUnits) : IRequest<IEnumerable<Project>>;


