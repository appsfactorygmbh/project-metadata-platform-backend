using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Represents a query to get projects by team numbers.
/// </summary>
/// <param name="TeamNumbers">A list of team numbers to filter the projects by.</param>
/// <returns>A request for an enumeration of projects that belong to the specified team numbers.</returns>
public record GetProjectsByTeamNumbersQuery(List<int> TeamNumbers) : IRequest<IEnumerable<Project>>;
