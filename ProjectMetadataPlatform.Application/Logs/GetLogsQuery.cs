using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

public record GetLogsQuery(int? ProjectId = null, string? Search = null): IRequest<List<Log>>;
