using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
///     Handler for the <see cref="CreatePluginCommand" />
/// </summary>
public class LoginQueryHandler : IRequestHandler<LoginQuery, JwtTokens>
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    ///     Creates a new instance of<see cref="LoginQueryHandler" />.
    /// </summary>
    /// <param name="authRepository"></param>
    public LoginQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    ///   Return the JWT tokens for the given login request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>JwtTokens when successful</returns>
    public async Task<JwtTokens> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        if (!_authRepository.CheckLogin(request.Username, request.Password).Result)
        {
            throw new InvalidOperationException("Invalid login credentials.");
        }
        //should also get this from the environmen
        /*
        var validIssuer = "ValidIssuer";
        var validAudience = "ValidAudience";
        var issuerSigningKey = new SymmetricSecurityKey("superSecretKeyThatIsAtLeast257BitLong@345"u8.ToArray());
        */

        var validIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER")
                          ?? throw new InvalidOperationException("JWT_VALID_ISSUER must be configured");
        var validAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? throw new InvalidOperationException("JWT_VALID_AUDIENCE must be configured");
        var issuerSigningKeyRaw = Environment.GetEnvironmentVariable("JWT_ISSUER_SIGNING_KEY")
                               ?? throw new InvalidOperationException("JWT_ISSUER_SIGNING_KEY must be configured");
        var issuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(issuerSigningKeyRaw));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, request.Username)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = validIssuer,
            Audience = validAudience,
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        //TODO save the refresh token in the database
        return new JwtTokens { AccessToken = stringToken, RefreshToken = Guid.NewGuid().ToString() };
    }
}
