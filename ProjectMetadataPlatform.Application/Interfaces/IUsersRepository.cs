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

}
