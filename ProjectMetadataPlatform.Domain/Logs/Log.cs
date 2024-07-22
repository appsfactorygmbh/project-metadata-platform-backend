using System;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Representation of a Log in database.
/// </summary>
public class Log
{
    /// <summary>
    ///     The id of the Log
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     the Username of the user taking action
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     The TImeStamp when the action was taken
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    ///     The Project, on wich the action was taken
    /// </summary>
    public Project? Project { get; set; } = null!;

    /// <summary>
    ///     The Project Id of the related Project
    /// </summary>
    public required int ProjectId { get; set; }

    /// <summary>
    ///     The taken action
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    ///     The changes that were made.
    /// </summary>
    public string Changes { get; set; }
}
