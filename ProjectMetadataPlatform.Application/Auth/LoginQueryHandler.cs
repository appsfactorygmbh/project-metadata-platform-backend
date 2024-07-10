using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Plugins;

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
        if (_authRepository.CheckLogin(request.Username, request.Password).Result)
        {
            throw new InvalidOperationException("Invalid login credentials.");
        }
        //should also get this from the environment
        var validIssuer = "ValidIssuer";
        var validAudience = "ValidAudience";
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, request.Username)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = validIssuer,
            Audience = validAudience,
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        return new JwtTokens{ AccessToken = stringToken, RefreshToken = Guid.NewGuid().ToString()};
    }
}
