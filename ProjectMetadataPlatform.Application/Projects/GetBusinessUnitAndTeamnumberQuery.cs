using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Represents a query to retrieve projects filtered by business unit and/or team number.
/// </summary>
/// <param name="BusinessUnit">The business unit to filter projects by. If null, the filter is ignored.</param>
/// <param name="TeamNumber">The team number to filter projects by. If null, the filter is ignored.</param>
/// <returns>A collection of projects that match the specified filters.</returns>
public record GetBusinessUnitAndTeamnumberQuery(string? BusinessUnit = null, int? TeamNumber = null) : IRequest<IEnumerable<Project>>;
