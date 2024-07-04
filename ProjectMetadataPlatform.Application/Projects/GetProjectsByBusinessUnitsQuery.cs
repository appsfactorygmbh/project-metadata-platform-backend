using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;
public record GetProjectsByBusinessUnitsQuery(List<string> BusinessUnits) : IRequest<IEnumerable<Project>>;


