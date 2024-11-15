using System;
using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Logs;

public record GetLogsQuery(int? ProjectId = null, string? Search = null): IRequest<IEnumerable<String>>;
