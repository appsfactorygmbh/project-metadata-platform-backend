using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;

    public UsersRepository(UserManager<User> userManager)
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
}
