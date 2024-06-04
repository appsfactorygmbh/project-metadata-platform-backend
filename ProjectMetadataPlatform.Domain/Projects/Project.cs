using System.Runtime.InteropServices;

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
    /// <param name="projectName">The name of the project.</param>
    /// <param name="clientName">The name of the client.</param>
    /// <param name="businessUnit">The Business Units id.</param>
    /// <param name="teamNumber">The team responsible for the project.</param>
    public Project(string projectName, string clientName, string businessUnit, int teamNumber) : this()
    {
        ProjectName = projectName;
        ClientName = clientName;
        BusinessUnit = businessUnit;
        TeamNumber = teamNumber;
    }

    public string ProjectName { get; set; }
    public string ClientName { get; set; }
    public string BusinessUnit { get; set; }
    public int TeamNumber { get; set; }
    


}
