using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Infrastructure.Users;

/// <summary>
///     The repository for users that handles the data access.
/// </summary>
public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="UsersRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    /// <param name="userManager">Manager for users of the type user.</param>
    public UsersRepository(ProjectMetadataPlatformDbContext dbContext, UserManager<User> userManager) : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;
    }

    /// <summary>
    ///     Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await GetEverything().ToListAsync();
    }

    /// <summary>
    /// Returns the user with the given username.
    /// </summary>
    /// <param name="userName">The username of the user to be searched for.</param>
    /// <returns>The user with the specified username, or null if not found.</returns>
    public Task<User?> GetUserByUserNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }

    /// <summary>
    /// Creates a new user with the given data.
    /// </summary>
    /// <param name="user">User to be created.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Id of the created User.</returns>
    public async Task<string> CreateUserAsync(User user, string password)
    {
        user.Id = ((_context.Users.Select(user => user.Id).ToList().Max(id => ((int?)int.Parse(id))) ?? 0) + 1).ToString();
        var identityResult = await _userManager.CreateAsync(user, password);

        return !identityResult.Succeeded ? throw new ArgumentException("User creation " + identityResult) : user.Id;
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
