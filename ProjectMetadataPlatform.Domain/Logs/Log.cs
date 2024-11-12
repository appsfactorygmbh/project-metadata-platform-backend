using System;
using System.Collections.Generic;
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
    /// Gets or sets the associated user.
    /// </summary>
    public User.User? User { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    public required string? UserId { get; set; }

    /// <summary>
    ///     the Username of the user taking action
    /// </summary>
    public required string? Username { get; set; }

    /// <summary>
    ///     The TImeStamp when the action was taken
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    ///     The Project, on wich the action was taken
    /// </summary>
    public Project? Project { get; set; }

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
    public List<LogChange>? Changes { get; set; }
}
