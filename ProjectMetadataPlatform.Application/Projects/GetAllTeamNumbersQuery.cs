using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetAllTeamNumbersQuery(): IRequest<IEnumerable<int>>;
