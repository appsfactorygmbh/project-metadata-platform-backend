using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Handles the query to retrieve logs based on project ID and search criteria.
/// </summary>
public class GetLogsQueryHandler: IRequestHandler<GetLogsQuery, IEnumerable<LogResponse>>
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
    public async Task<IEnumerable<LogResponse>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        List<Log> logs = request.ProjectId != null
            ? await _logRepository.GetLogsForProject((int)request.ProjectId!)
            : await _logRepository.GetAllLogs();
        IEnumerable<LogResponse> logResponses = logs.Select(BuildLogMessage);

        if (request.Search != null)
        {
            return logResponses.Where(log => log.LogMessage.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        return logResponses;
    }

    /// <summary>
    /// Builds a log message from a log entry.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>A log response containing the message and timestamp.</returns>
    private static LogResponse BuildLogMessage(Log log)
    {
        string message;

        if (log.User is { UserName: not null })
        {
            message = log.User.UserName;
        }
        else
        {
            message = log.Username ?? "<Deleted User>";
        }

        message += " " + log.Action switch
        {
            Action.ADDED_PROJECT => BuildAddedProjectMessage(log.Changes),
            Action.UPDATED_PROJECT => BuildUpdatedProjectMessage(log.Changes),
            Action.ARCHIVED_PROJECT => BuildArchivedProjectMessage(log.Project?.ProjectName),
            Action.UNARCHIVED_PROJECT => BuildUnArchivedProjectMessage(log.Project?.ProjectName),
            Action.ADDED_PROJECT_PLUGIN => BuildAddedProjectPluginMessage(log.Project?.ProjectName, log.Changes),
            Action.UPDATED_PROJECT_PLUGIN => BuildUpdatedProjectPluginMessage(log.Project?.ProjectName, log.Changes),
            Action.REMOVED_PROJECT_PLUGIN => BuildRemovedProjectPluginMessage(log.Project?.ProjectName, log.Changes),
            _ => ""
        };

        return new LogResponse(message, GetTimestamp(log.TimeStamp));
    }

    /// <summary>
    /// Builds a message for an added project.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedProjectMessage(List<LogChange>? changes) {
        var message = "created a new project";
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an updated project.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedProjectMessage(List<LogChange>? changes) {
        var message = "updated project properties: ";
        if (changes == null) {
            return message;
        }
        message += string.Join(", ", changes.Select(change => $" set {change.Property} from {change.OldValue} to {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an archived project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildArchivedProjectMessage(string? projectName) {
        return "archived project " + (projectName ?? "<Unknown Project>");
    }

    /// <summary>
    /// Builds a message for an unarchived project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUnArchivedProjectMessage(string? projectName) {
        return "unarchived project " + (projectName ?? "<Unknown Project>");
    }

    /// <summary>
    /// Builds a message for an added project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        var message = "added a new plugin to project " + (projectName ?? "<Unknown Project>");
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an updated project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        var message = "updated plugin properties in project " + (projectName ?? "<Unknown Project>") + ": ";
        if (changes == null) {
            return message;
        }
        message += string.Join(", ", changes.Select(change => $" set {change.Property} from {change.OldValue} to {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for a removed project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        var message = "removed a plugin from project " + (projectName ?? "<Unknown Project>");
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Gets the timestamp in a specific format.
    /// </summary>
    /// <param name="value">The DateTimeOffset value.</param>
    /// <returns>The formatted timestamp.</returns>
    private static string GetTimestamp(DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-ddTHH:mm:ssK");
    }
}
