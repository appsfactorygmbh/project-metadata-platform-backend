using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.User;

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
    /// Returns the user with the given id.
    /// </summary>
    /// <param name="id">Id of the searched for user.</param>
    /// <returns>User that is searched for or null.</returns>
    Task<User?> GetUserByIdAsync(int id);



}
