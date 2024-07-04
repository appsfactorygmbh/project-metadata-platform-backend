using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetBusinessUnitAndTeamnumberQuery(string? BusinessUnit = null, int? TeamNumber = null) : IRequest<IEnumerable<Project>>;
