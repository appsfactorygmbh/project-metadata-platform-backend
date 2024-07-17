using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application;
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
    /// <param name="dbContext"></param>
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
        return user != null && await _userManager.CheckPasswordAsync(user, password);
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
    /// <summary>
    /// Saves a refresh Token to the database.
    /// </summary>
    /// <param name="username">associated Username</param>
    /// <param name="refreshToken">Value of the Token</param>
    /// <returns></returns>
    public async Task StoreRefreshToken(string username, string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName == username);
        var expirationTime = int.Parse(EnvironmentUtils.GetEnvVarOrLoadFromFile("REFRESH_TOKEN_EXPIRATION_HOURS"));
        var token = new RefreshToken
        {

            Token = refreshToken,
            User = user,
            UserId = user?.Id,
            ExpirationDate = DateTime.UtcNow.AddHours(expirationTime)

        };
        Create(token);
        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// updates an existing refresh Token.
    /// </summary>
    /// <param name="username">associates Username</param>
    /// <param name="refreshToken">Values of the Token</param>
    /// <returns></returns>
    public async Task UpdateRefreshToken(string username, string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName == username);
        var token = GetIf(rt => user != null && rt.UserId == user.Id).FirstOrDefaultAsync().Result;
        var expirationTime = int.Parse(EnvironmentUtils.GetEnvVarOrLoadFromFile("REFRESH_TOKEN_EXPIRATION_HOURS"));
        if (token != null)
        {
            token.Token = refreshToken;
            token.ExpirationDate = DateTime.UtcNow.AddHours(expirationTime);
            Update(token);
        }

        _ = await _context.SaveChangesAsync();

    }

    /// <summary>
    /// Checks for the existence of a refresh Token for a specific user.
    /// </summary>
    /// <param name="username">name of a user</param>
    /// <returns>True if a token exists; False if no token exists</returns>
    public async Task<bool> CheckRefreshTokenExists(string username)
    {

        return GetIf(rt => rt.User != null && rt.User.UserName == username).Any();
    }

    /// <summary>
    /// Checks if a refresh Token is valid.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns>true if the token is valid; false if the token isn't valid</returns>
    public async Task<bool> CheckRefreshTokenRequest(string refreshToken)
    {
        var token = GetIf(rt => rt.Token == refreshToken).FirstOrDefaultAsync().Result;

        return (token != null  && token.ExpirationDate > DateTime.UtcNow);
    }

    /// <summary>
    /// Gets the username related to a refresh Token.
    /// </summary>
    /// <param name="refreshToken">a refresh Token</param>
    /// <returns>a username</returns>
    public async Task<string?> GetUserNamebyRefreshToken(string refreshToken)
    {
        var token = GetIf(rt => rt.Token == refreshToken).FirstOrDefaultAsync().Result;
        var user = await _userManager.Users.FirstOrDefaultAsync(a => token != null && a.Id == token.UserId);

        return user?.UserName;
    }


}
