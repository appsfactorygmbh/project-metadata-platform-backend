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

public class GetLogsQueryHandler: IRequestHandler<GetLogsQuery, IEnumerable<String>>
{
    private readonly ILogRepository _logRepository;

    public GetLogsQueryHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<IEnumerable<String>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        List<Log> logs;
        if (request.ProjectId != null)
        {
            logs = await _logRepository.GetLogsForProject((int)request.ProjectId!);
        }
        else
        {
            logs = await _logRepository.GetAllLogs();
        }

        return logs.Select(BuildLogMessage);
    }

    private static string BuildLogMessage(Log log)
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

        return log.Action switch
        {
            Action.ADDED_PROJECT => $"{message} {BuildAddedProjectMessage(log.Changes)}",
            Action.UPDATED_PROJECT => $"{message} {BuildUpdatedProjectMessage(log.Changes)}",
            Action.ARCHIVED_PROJECT => $"{message} {BuildArchivedProjectMessage(log.Project?.ProjectName)}",
            Action.ADDED_PROJECT_PLUGIN => $"{message} {BuildAddedProjectPluginMessage(log.Project?.ProjectName, log.Changes)}",
            Action.UPDATED_PROJECT_PLUGIN => $"{message} {BuildUpdatedProjectPluginMessage(log.Project?.ProjectName, log.Changes)}",
            Action.REMOVED_PROJECT_PLUGIN => $"{message} {BuildRemovedProjectPluginMessage(log.Project?.ProjectName, log.Changes)}",
            _ => message
        };
    }

    private static string BuildAddedProjectMessage(List<LogChange>? changes) {
        var message = "created a new project";
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    private static string BuildUpdatedProjectMessage(List<LogChange>? changes) {
        var message = "updated project properties: ";
        if (changes == null) {
            return message;
        }
        message += string.Join(", ", changes.Select(change => $" set {change.Property} from {change.OldValue} to {change.NewValue}"));
        return message;
    }

    private static string BuildArchivedProjectMessage(string? projectName) {
        return "archived project " + (projectName ?? "<Unknown Project>");
    }

    private static string BuildAddedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        string message = "added a new plugin to project " + (projectName ?? "<Unknown Project>");
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    private static string BuildUpdatedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        string message = "updated plugin properties in project " + (projectName ?? "<Unknown Project>") + ": ";
        if (changes == null) {
            return message;
        }
        message += string.Join(", ", changes.Select(change => $" set {change.Property} from {change.OldValue} to {change.NewValue}"));
        return message;
    }

    private static string BuildRemovedProjectPluginMessage(string? projectName, List<LogChange>? changes) {
        string message = "removed a plugin from project " + (projectName ?? "<Unknown Project>");
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    private static string GetTimestamp(DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-ddTHH:mm:ssK");
    }
}
