using System;
using System.Collections.Generic;
using System.Linq;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Logs;

/// <inheritdoc />
public class LogConverter: ILogConverter
{
    // TODO keep in sync with Action enum and the LogRepository in the Infrastructure project

    /// <inheritdoc />
    public LogResponse BuildLogMessage(Log log)
    {
        var message = log.User is { Email: not null }
            ? log.User.Email
            : log.Email != null ? log.Email + " (deleted user)" : "<Deleted User>";

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
