using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.User;
using System.Collections.Generic;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for user data.
/// </summary>
public interface IUsersRepository
{
    /// <summary>
    /// Creates a new user with the given data.
    /// </summary>
    /// <param name="user">User to be created.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Id of the created user.</returns>
    Task<string> CreateUserAsync(User user, string password);

    /// <summary>
    /// Returns all users.
    /// </summary>
    /// <returns>Enumerable of all User-Objects</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Returns a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The User object if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// Returns the user with the given username.
    /// </summary>
    /// <param name="userName">The username of the searched for user.</param>
    /// <returns>The user that is searched for or null.</returns>
    Task<User?> GetUserByUserNameAsync(string userName);

    /// <summary>
    /// Stores a user.
    /// </summary>
    /// <param name="user">The User object to store.</param>
    /// <returns>The stored User object.</returns>
    Task<User> StoreUser(User user);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="user">The user to be deleted.</param>
    /// <returns>The deleted user.</returns>
    Task<User>DeleteUserAsync(User user);

    Task<User?> GetUserByEmailAsync(string email);
}
