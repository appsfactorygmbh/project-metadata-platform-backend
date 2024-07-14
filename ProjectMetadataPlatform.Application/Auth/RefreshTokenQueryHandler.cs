using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, JwtTokens>
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    ///     Creates a new instance of<see cref="RefreshQueryHandler" />.
    /// </summary>
    /// <param name="authRepository"></param>
    public RefreshTokenQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    public async Task<JwtTokens> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var principal = new JwtSecurityTokenHandler().ValidateToken(request.AccessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(TokenDescriptorInformation.ReadFromEnvVariables().IssuerSigningKey)),
            ValidateIssuer = true,
            ValidIssuer = TokenDescriptorInformation.ReadFromEnvVariables().ValidIssuer,
            ValidateAudience = true,
            ValidAudience = TokenDescriptorInformation.ReadFromEnvVariables().ValidAudience,
            ValidateLifetime = false

        }, out _);

        if (principal?.Identity?.Name is null)
        {
            throw new AuthenticationException("Invalid token.");
        }
        if (!_authRepository.CheckRefreshTokenRequest(principal.Identity.Name, request.RefreshToken).Result)
        {
            throw new AuthenticationException("Invalid refresh token.");
        }
        var tokenDescriptorInformation = TokenDescriptorInformation.ReadFromEnvVariables();
        var issuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenDescriptorInformation.IssuerSigningKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, principal.Identity.Name)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = tokenDescriptorInformation.ValidIssuer,
            Audience = tokenDescriptorInformation.ValidAudience,
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        return new JwtTokens { AccessToken = stringToken, RefreshToken = request.RefreshToken};
    }
}
