using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public record GetProjectsBySearchingQuery(string search): IRequest<IEnumerable<Project>>;
