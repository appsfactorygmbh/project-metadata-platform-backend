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
        var tokenDescriptorInformation = await _authRepository.GetTokenDescriptorInformation();
        var issuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenDescriptorInformation!.IssuerSigningKey!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, request.Username)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = tokenDescriptorInformation.ValidIssuer,
            Audience = tokenDescriptorInformation.ValidAudience,
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        //TODO save the refresh token in the database
        return new JwtTokens { AccessToken = stringToken, RefreshToken = Guid.NewGuid().ToString() };
    }
}
