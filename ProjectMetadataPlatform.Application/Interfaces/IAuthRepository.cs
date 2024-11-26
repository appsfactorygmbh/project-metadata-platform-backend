
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

    /// <summary>
    /// Saves a refresh Token to the database.
    /// </summary>
    /// <param name="username">associated Username</param>
    /// <param name="refreshToken">Value of the Token</param>
    /// <returns></returns>
    Task StoreRefreshToken(string username, string refreshToken);

    /// <summary>
    /// updates an existing refresh Token.
    /// </summary>
    /// <param name="username">associates Username</param>
    /// <param name="refreshToken">Values of the Token</param>
    /// <returns></returns>
    Task UpdateRefreshToken(string username, string refreshToken);

    /// <summary>
    /// Checks for the existence of a refresh Token for a specific user.
    /// </summary>
    /// <param name="username">name of a user</param>
    /// <returns>True if a token exists; False if no token exists</returns>
    Task<bool> CheckRefreshTokenExists(string username);

    /// <summary>
    /// Checks if a refresh Token is valid.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns>true if the token is valid; false if the token isn't valid</returns>
    Task<bool> CheckRefreshTokenRequest(string refreshToken);

    /// <summary>
    /// Gets the email related to a refresh Token.
    /// </summary>
    /// <param name="refreshToken">a refresh Token</param>
    /// <returns>a email</returns>
    Task<string?> GetEmailByRefreshToken(string refreshToken);
}
