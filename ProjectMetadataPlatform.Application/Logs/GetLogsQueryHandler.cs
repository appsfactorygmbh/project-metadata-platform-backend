using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

public class GetLogsQueryHandler: IRequestHandler<GetLogsQuery, List<Log>>
{
    private readonly ILogRepository _logRepository;

    public GetLogsQueryHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public Task<List<Log>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        return _logRepository.GetAllLogs();
    }
}
