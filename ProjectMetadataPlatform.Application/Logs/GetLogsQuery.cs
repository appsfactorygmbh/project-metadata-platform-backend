using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

public record GetLogsQuery(int ProjectId, string Search): IRequest<List<Log>>;
