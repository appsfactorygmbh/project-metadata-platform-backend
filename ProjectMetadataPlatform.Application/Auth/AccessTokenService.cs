using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ProjectMetadataPlatform.Application.Auth;

public static class AccessTokenService
{
    public static string CreateAccessToken(string username)
    {
        var tokenDescriptorInformation = TokenDescriptorInformation.ReadFromEnvVariables();
        var issuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenDescriptorInformation.IssuerSigningKey));
        var expirationTime = int.Parse(EnvironmentUtils.GetEnvVarOrLoadFromFile("ACCESS_TOKEN_EXPIRATION_MINUTES"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, username)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(expirationTime),
            Issuer = tokenDescriptorInformation.ValidIssuer,
            Audience = tokenDescriptorInformation.ValidAudience,
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
