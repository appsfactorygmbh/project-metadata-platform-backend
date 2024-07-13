using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Handles User Management using the UserManager provided by AspNetCore Identity.
/// </summary>
public class AuthRepository : RepositoryBase<RefreshToken>,IAuthRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="userManager"></param>
    public AuthRepository(ProjectMetadataPlatformDbContext dbContext,UserManager<IdentityUser> userManager) : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;

    }

    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True, if the credentials are correct</returns>
    public async Task<bool> CheckLogin(string username, string password)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName == username);
        return await _userManager.CheckPasswordAsync(user!, password);
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
    public async Task StoreRefreshToken(string username, string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName == username);
        var RefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddHours(6)

        };
        Create(RefreshToken);
        _ = await _context.SaveChangesAsync();
    }



}
