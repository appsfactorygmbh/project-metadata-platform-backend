using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Infrastructure.Users;

/// <summary>
///     The repository for users that handles the data access.
/// </summary>
public class UsersRepository : RepositoryBase<IdentityUser>, IUsersRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="UsersRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    /// <param name="userManager">Manager for users of the type user.</param>
    public UsersRepository(ProjectMetadataPlatformDbContext dbContext,UserManager<IdentityUser> userManager) : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;
    }

    /// <summary>
    ///     Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
    {
        return await GetEverything().ToListAsync();
    }

    /// <summary>
    /// Returns the user with the given email.
    /// </summary>
    /// <param name="email">The email of the user to be searched for.</param>
    /// <returns>The user with the specified email, or null if not found.</returns>
    public Task<IdentityUser?> GetUserByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Creates a new user with the given data.
    /// </summary>
    /// <param name="user">User to be created.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Id of the created User.</returns>
    public async Task<string> CreateUserAsync(IdentityUser user, string password)
    {
        var userIds = await _context.Users
            .Select(u => u.Id)
            .ToListAsync();

        var maxId = userIds
            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : 0)
            .DefaultIfEmpty(0)
            .Max();

        user.Id = (maxId + 1).ToString(CultureInfo.InvariantCulture);

        var identityResult = await _userManager.CreateAsync(user, password);
        return identityResult.Errors.Any(e => e.Code == "DuplicateUserName")
            ? throw new ArgumentException("User creation Failed : DuplicateEmail")
            : !identityResult.Succeeded ? throw new ArgumentException("User creation " + identityResult) : user.Id;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user with the specified identifier, or null if not found.</returns>
    public Task<IdentityUser?> GetUserByIdAsync(string id)
    {
        return _userManager.FindByIdAsync(id);
    }

    /// <summary>
    /// Stores the user information.
    /// </summary>
    /// <param name="user">The user to be stored.</param>
    /// <returns>The stored user.</returns>
    public async Task<IdentityUser> StoreUser(IdentityUser user)
    {
        var identityResult = user.Id == "" ? await _userManager.CreateAsync(user) : await _userManager.UpdateAsync(user);

        return identityResult.Errors.Any(e => e.Code == "DuplicateUserName")
            ? throw new ArgumentException("User creation Failed : DuplicateEmail")
            : !identityResult.Succeeded ? throw new ArgumentException("User creation " + identityResult) : user;
    }

    /// <summary>
    /// Deletes the specified user asynchronously.
    /// </summary>
    /// <param name="user">The user to be deleted.</param>
    /// <returns>The task result contains the deleted user.</returns>
    public async Task<IdentityUser> DeleteUserAsync(IdentityUser user)
    {
        // Remove all refresh tokens of the user
        var refreshTokens = ProjectMetadataPlatformDbContext.Set<RefreshToken>();
        refreshTokens.RemoveRange(refreshTokens.Where(rt => rt.UserId == user.Id));

        var task = await _userManager.DeleteAsync(user);
        return !task.Succeeded ? throw new ArgumentException("User deletion failed. With id " + user.Id + task) : user;
    }

    public async Task<bool> CheckPasswordFormat(string password)
    {
        var passwordValidator = new PasswordValidator<IdentityUser>();
        var identityResult = await passwordValidator.ValidateAsync(_userManager, null, password);
        return !identityResult.Succeeded ? throw new ArgumentException("User creation " + identityResult) : true;

    }
}
