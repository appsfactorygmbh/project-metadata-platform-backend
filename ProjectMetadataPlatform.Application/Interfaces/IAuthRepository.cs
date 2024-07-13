using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for authentication.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True, if the credentials are correct</returns>
    Task<bool> CheckLogin(string username, string password);

    /// <summary>
    /// Creates a new user with the given username and password.
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns></returns>
    Task<string?> CreateUser(string username, string password);

    Task StoreRefreshToken(string userId, string refreshToken);

    Task<string> GetUserId(string username);


}
