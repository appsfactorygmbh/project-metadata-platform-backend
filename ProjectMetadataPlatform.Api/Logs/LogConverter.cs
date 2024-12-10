using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
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
        var message = log.Author is { Email: not null }
            ? log.Author.Email
            : log.AuthorEmail != null ? log.AuthorEmail + " (deleted user)" : "<Deleted User>";

        message += " " + log.Action switch
        {
            Action.ADDED_PROJECT => BuildAddedProjectMessage(log.Changes),
            Action.UPDATED_PROJECT => BuildUpdatedProjectMessage(log.Changes),
            Action.ARCHIVED_PROJECT => BuildArchivedProjectMessage(log.ProjectName),
            Action.UNARCHIVED_PROJECT => BuildUnArchivedProjectMessage(log.ProjectName),
            Action.ADDED_PROJECT_PLUGIN => BuildAddedProjectPluginMessage(log.ProjectName, log.Changes),
            Action.UPDATED_PROJECT_PLUGIN => BuildUpdatedProjectPluginMessage(log.ProjectName, log.Changes),
            Action.REMOVED_PROJECT_PLUGIN => BuildRemovedProjectPluginMessage(log.ProjectName, log.Changes),
            Action.ADDED_USER => BuildAddedUserMessage(log.Changes),
            Action.UPDATED_USER => BuildUpdatedUserMessage(log),
            Action.REMOVED_USER => BuildRemovedUserMessage(log.AffectedUserEmail ?? "<Unknown User>"),
            Action.REMOVED_PROJECT => BuildRemovedProjectMessage(log.ProjectName ?? "<Unknown Project>"),
            Action.ADDED_GLOBAL_PLUGIN => BuildAddedGlobalPluginMessage(log.Changes),
            Action.UPDATED_GLOBAL_PLUGIN => BuildUpdatedGlobalPluginMessage(log.Changes, log.GlobalPluginName ?? "<Unknown Plugin>"),
            Action.ARCHIVED_GLOBAL_PLUGIN => BuildArchivedGlobalPluginMessage(log.GlobalPluginName ?? "<Unknown Plugin>"),
            Action.UNARCHIVED_GLOBAL_PLUGIN => BuildUnArchivedGlobalPluginMessage(log.GlobalPluginName?? "<Unknown Plugin>"),
            Action.REMOVED_GLOBAL_PLUGIN => BuildRemovedGlobalPluginMessage(log.GlobalPluginName ?? "<Unknown Plugin>"),
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
    /// Builds a message for an added user.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedUserMessage(List<LogChange>? changes) {
        var message = "added a new user";
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an updated user.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedUserMessage(Log log) {
        var affectedUserEmail = log.AffectedUserEmail ?? "<Unknown User>";

        var message = $"updated user {affectedUserEmail}: ";
        message += string.Join(", ", log.Changes!.Select(change =>
            change.Property switch
            {
                nameof(IdentityUser.PasswordHash) => "changed password",
                _ => $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            }));
        return message;
    }

    /// <summary>
    /// Builds a message for a removed user.
    /// </summary>
    /// <param name="username">The username of the removed user.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedUserMessage(string username) {
        return "removed user " + username;
    }

    /// <summary>
    /// Builds a message for a removed project.
    /// </summary>
    /// <param name="projectName">The name of the removed project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedProjectMessage(string projectName) {
        return "removed project " + projectName;
    }

    /// <summary>
    /// Builds a message for an added global plugin.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedGlobalPluginMessage(List<LogChange>? changes) {
        var message = "added a new global plugin";
        if (changes == null) {
            return message;
        }
        message += " with properties: ";
        message += string.Join(", ", changes.Select(change => $"{change.Property} = {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an updated global plugin.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <param name="pluginName">The name of the updated plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedGlobalPluginMessage(List<LogChange>? changes, string pluginName) {
        var message = $"updated global plugin {pluginName}: ";
        if (changes == null) {
            return message;
        }
        message += string.Join(", ", changes.Select(change => $"set {change.Property} from {change.OldValue} to {change.NewValue}"));
        return message;
    }

    /// <summary>
    /// Builds a message for an archived global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the archived global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildArchivedGlobalPluginMessage(string pluginName) {
        return "archived global plugin " + pluginName;
    }

    /// <summary>
    /// Builds a message for an unarchived global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the unarchived global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUnArchivedGlobalPluginMessage(string pluginName) {
        return "unarchived global plugin " + pluginName;
    }

    /// <summary>
    /// Builds a message for a removed global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the removed global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedGlobalPluginMessage(string pluginName) {
        return "removed global plugin " + pluginName;
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
