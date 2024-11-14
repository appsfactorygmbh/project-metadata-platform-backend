using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Logs;

[ApiController]
// [Authorize]
[Route("[controller]")]
public class LogsController: ControllerBase
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetLogResponse>>> Get()
    {
        var query = new GetLogsQuery(0, "");

        List<Log> logs;

        try
        {
            logs = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var result = logs.Select(l => new GetLogResponse(BuildLogMessage(l), GetTimestamp(l.TimeStamp)));
        return Ok(result);
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
        string message = "created a new project";
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    private static string BuildUpdatedProjectMessage(List<LogChange>? changes) {
        string message = "updated project properties: ";
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
