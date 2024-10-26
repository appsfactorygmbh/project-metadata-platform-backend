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
    /// <returns></returns>
    Task CreateUserAsync(User user, string password);

    Task<User?> GetUserByIdAsync(int id);



}
