using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Handles the query to retrieve logs based on project ID and search criteria.
/// </summary>
public class GetLogsQueryHandler: IRequestHandler<GetLogsQuery, IEnumerable<Log>>
{
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLogsQueryHandler"/> class.
    /// </summary>
    /// <param name="logRepository">The log repository instance.</param>
    public GetLogsQueryHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    /// <summary>
    /// Handles the GetLogsQuery request.
    /// </summary>
    /// <param name="request">The request containing project ID and search criteria.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of log responses.</returns>
    public async Task<IEnumerable<Log>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        // TODO reimplement search
        List<Log> logs = request.ProjectId != null
            ? await _logRepository.GetLogsForProject((int)request.ProjectId!)
            : await _logRepository.GetAllLogs();
        return logs;
    }
}
