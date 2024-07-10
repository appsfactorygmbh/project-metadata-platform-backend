using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Handles User Management using the UserManager provided by AspNetCore Identity.
/// </summary>
public class AuthRepository :  IAuthRepository
{

    private readonly UserManager<IdentityUser> _userManager;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="userManager"></param>
    public AuthRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>True, if the credentials are correct</returns>
    public async Task<bool> CheckLogin(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        return (user != null) && (await _userManager.CheckPasswordAsync(user, password));
    }

    /// <summary>
    /// Creates a new user with the given username and password.
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns></returns>
    public async Task<string?> CreateUser(string username, string password)
    {
        var user = new IdentityUser { UserName = username };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded ? user.Id : null;
    }
}
