using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Represents a query to retrieve all distinct team numbers from projects.
/// </summary>
/// <remarks>
/// This query does not require any parameters, making it straightforward to use for fetching a list of unique team numbers.
/// It leverages the MediatR library for CQRS pattern implementation, ensuring a clean separation of concerns and scalability.
/// </remarks>
public record GetAllTeamNumbersQuery() : IRequest<IEnumerable<int>>;
