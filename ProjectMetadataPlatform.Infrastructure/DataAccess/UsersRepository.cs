using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    private readonly UserManager<User> _userManager;

    public UsersRepository(ProjectMetadataPlatformDbContext projectMetadataPlatformDbContext, UserManager<User> userManager) : base(projectMetadataPlatformDbContext)
    {
        _userManager = userManager;
    }

    public Task<User?> GetUserByIdAsync(int id)
    {
        return _userManager.FindByIdAsync(id.ToString());
    }

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

    /// <summary>
    /// Creates a new user with the given username and password.
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns></returns>
    public async Task<string?> CreateUser(string username, string password)
    {
        var user = new User { UserName = username };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded ? user.Id : null;
    }
}
