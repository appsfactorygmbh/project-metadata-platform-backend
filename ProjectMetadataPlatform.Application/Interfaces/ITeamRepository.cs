using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for authentication.
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Retrieves the name of the team specified by the given id.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>The name of the team specified by the id.</returns>
    Task<string> RetrieveNameForId(int id);

    /// <summary>
    /// Checks if a team exists.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>True if the team with the given id is present.</returns>
    Task<bool> CheckIfTeamExists(int id);
}
