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
        _projectName = projectName;
        _clientName = clientName;
        _businessUnit = businessUnit;
        _teamNumber = teamNumber;
    }
    
    private string _projectName;
    public string ProjectName
    {
        get => _projectName;
        set => _projectName = value;
    }
    
    private string _clientName;
    public string ClientName
    {
        get => _clientName;
        set => _clientName = value;
    }
    
    private string _businessUnit;
    public string BusinessUnit 
    {
        get => _businessUnit;
        set => _businessUnit = value;
    }
    
    private int _teamNumber;
    public int TeamNumber
    {
        get => _teamNumber;
        set => _teamNumber = value;
    }
}
