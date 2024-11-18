namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Enum for log changes actions
/// </summary>
public enum Action
{
    /// <summary>
    /// Represents the action of adding a project.
    /// </summary>
    ADDED_PROJECT,

    /// <summary>
    /// Represents the action of adding a project plugin.
    /// </summary>
    ADDED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of updating a project.
    /// </summary>
    UPDATED_PROJECT,

    /// <summary>
    /// Represents the action of updating a project plugin.
    /// </summary>
    UPDATED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of removing a project plugin.
    /// </summary>
    REMOVED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of archiving a project.
    /// </summary>
    ARCHIVED_PROJECT,

    /// <summary>
    /// Represents the action of unarchiving a project.
    /// </summary>
    UNARCHIVED_PROJECT
}
