namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Enum for log changes actions
/// </summary>
public enum Action
{
    /// <summary>
    ///     Something was added
    /// </summary>
    ADDED_PROJECT,
    ADDED_PROJECT_PLUGIN,

    /// <summary>
    ///     Something in the project was updated.
    /// </summary>
    UPDATED_PROJECT,
    UPDATED_PROJECT_PLUGIN,

    /// <summary>
    ///     The project or something in it was removed
    /// </summary>
    REMOVED_PROJECT_PLUGIN,
    ARCHIVED_PROJECT
}
