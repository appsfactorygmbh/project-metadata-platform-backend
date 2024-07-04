using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
///     The representation of a project in the Database.
/// </summary>
// TODO Add list of plugins as property.
public class Project
{
    /// <summary>
    ///     Gets or sets the id of the project.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the project. This property is required.
    /// </summary>
    public required string ProjectName { get; set; }

    /// <summary>
    ///     Gets or sets the name of the client associated with the project. This property is required.
    /// </summary>
    public required string ClientName { get; set; }

    /// <summary>
    ///     Gets or sets the business unit associated with the project. This property is required.
    /// </summary>
    public required string BusinessUnit { get; set; }

    /// <summary>
    ///     Gets or sets the team number associated with the project. This property is required.
    /// </summary>
    public required int TeamNumber { get; set; }

    /// <summary>
    ///     Gets or sets the department associated with the project. This property is required.
    /// </summary>
    public required string Department { get; set; }

    /// <summary>
    ///     Is used for the many-to-many relation in EF core.
    /// </summary>
    public ICollection<ProjectPlugins>? ProjectPlugins { get; set; }
}
