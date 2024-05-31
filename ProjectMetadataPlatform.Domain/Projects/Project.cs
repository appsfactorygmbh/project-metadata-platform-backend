using System.Runtime.InteropServices;

namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Project properties.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client.</param>
/// <param name="BusinessUnit">The Business Units id.</param>
/// <param name="TeamNumber">The team responsible for the project.</param>
// TODO Add list of plugins as property.
public class Project()
{
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
