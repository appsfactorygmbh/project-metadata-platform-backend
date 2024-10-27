using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Repository for user data.
/// </summary>
public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    /// <param name="userManager">Manager for users of the type user.</param>
    public UsersRepository(ProjectMetadataPlatformDbContext dbContext,UserManager<User> userManager) : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;
    }

    /// <summary>
    /// Creates a new user with the given data.
    /// </summary>
    /// <param name="user">User to be created.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Id of the created User.</returns>
    public async Task<string> CreateUserAsync(User user, string password)
    {
        var identityResult = await _userManager.CreateAsync(user, password);
        _ = await _context.SaveChangesAsync();
        return !identityResult.Succeeded ? throw new ArgumentException("User creation "+identityResult) : user.Id;
    }

    /// <summary>
    /// Returns the user with the given id.
    /// </summary>
    /// <param name="id">Id of the user to be searched for.</param>
    /// <returns>If found the user otherwise null.</returns>
    public Task<User?> GetUserByIdAsync(int id)
    {
        return _userManager.FindByIdAsync(id.ToString());
    }
}
