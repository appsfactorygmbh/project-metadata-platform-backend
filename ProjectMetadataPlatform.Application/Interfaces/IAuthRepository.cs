using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for authentication.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>True, if the credentials are correct</returns>
    Task<bool> CheckLogin(string username, string password);
}
