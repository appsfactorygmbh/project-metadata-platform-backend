using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Represents a query to retrieve all business units within the system.
/// </summary>
public record GetAllBusinessUnitsQuery : IRequest<IEnumerable<string>>;
