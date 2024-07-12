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
    /// <summary>
    /// Get the information for the token descriptor.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TokenDescriptorInformation?> GetTokenDescriptorInformation()
    {
        var validIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER")
                          ?? throw new InvalidOperationException("JWT_VALID_ISSUER must be configured");
        var validAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE")
                            ?? throw new InvalidOperationException("JWT_VALID_AUDIENCE must be configured");
        var issuerSigningKey = Environment.GetEnvironmentVariable("JWT_ISSUER_SIGNING_KEY")
                                  ?? throw new InvalidOperationException("JWT_ISSUER_SIGNING_KEY must be configured");

        return new TokenDescriptorInformation
        {
            ValidIssuer = validIssuer, ValidAudience = validAudience, IssuerSigningKey = issuerSigningKey
        };
    }
}
