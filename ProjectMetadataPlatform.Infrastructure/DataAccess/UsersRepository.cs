using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Repository for accessing and storing user data.
/// </summary>
public class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersRepository"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for handling user operations.</param>
    public UsersRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user with the specified identifier, or null if not found.</returns>
    public Task<User?> GetUserByIdAsync(string id)
    {
        return _userManager.FindByIdAsync(id);
    }

    /// <summary>
    /// Stores the user information.
    /// </summary>
    /// <param name="user">The user to be stored.</param>
    /// <returns>The stored user.</returns>
    public async Task<User> StoreUser(User user)
    {
        if (user.Id == "")
        {
            await _userManager.CreateAsync(user);
        }
        else
        {
            await _userManager.UpdateAsync(user);
        }

        return user;
    }
}
