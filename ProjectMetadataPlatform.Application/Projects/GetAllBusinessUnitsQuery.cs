using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetAllBusinessUnitsQuery() : IRequest<IEnumerable<string>>;
