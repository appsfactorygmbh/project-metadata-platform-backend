using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class UsersRepository : RepositoryBase<IdentityUser>, IUsersRepository
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersRepository(ProjectMetadataPlatformDbContext projectMetadataPlatformDbContext, UserManager<IdentityUser> userManager) : base(projectMetadataPlatformDbContext)
    {
        _userManager = userManager;
    }

    public Task<IdentityUser?> GetUserByIdAsync(int id)
    {
        return _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<IdentityUser> StoreUser(IdentityUser user)
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
