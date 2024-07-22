namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Enum for log changes actions
/// </summary>
public enum Action
{
    /// <summary>
    ///     Something was added
    /// </summary>
    ADDED,

    /// <summary>
    ///     Something in the project was updated.
    /// </summary>
    UPDATED,

    /// <summary>
    ///     The project or something in it was removed
    /// </summary>
    REMOVED
}
