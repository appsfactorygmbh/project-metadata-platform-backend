

namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// The representation of a project in the Database.
/// </summary>
// TODO Add list of plugins as property.
public class Project()
{
    /// <summary>
    /// Project properties.
    /// </summary>
    /// <param name="id">Internal ID of the project</param>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="clientName">The name of the client.</param>
    /// <param name="businessUnit">The Business Units id.</param>
    /// <param name="teamNumber">The team responsible for the project.</param>
    /// <param name="department">The departments id.</param>
    public Project(int id, string projectName, string clientName, string businessUnit, int teamNumber, string department) : this()
    {
        Id = id;
        ProjectName = projectName;
        ClientName = clientName;
        BusinessUnit = businessUnit;
        TeamNumber = teamNumber;
        Department = department;
    }
    /// <summary>
    /// Gets or sets the internal ID of the project. This property is required.
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the project. This property is required.
    /// </summary>
    public required string ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the name of the client associated with the project. This property is nullable.
    /// </summary>
    public required string ClientName { get; set; }

    /// <summary>
    /// Gets or sets the business unit associated with the project. This property is nullable.
    /// </summary>
    public required string BusinessUnit { get; set; }

    /// <summary>
    /// Gets or sets the team number associated with the project. This property is required.
    /// </summary>
    public required int TeamNumber { get; set; }

    /// <summary>
    /// Gets or sets the department associated with the project. This property is nullable.
    /// </summary>
    public required string Department { get; set; }


    
}
