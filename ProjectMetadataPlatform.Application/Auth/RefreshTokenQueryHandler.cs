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

/// <summary>
/// Handler for the <see cref="RefreshTokenQuery" />
/// </summary>
public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, JwtTokens>
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    ///     Creates a new instance of<see cref="RefreshTokenQueryHandler" />.
    /// </summary>
    /// <param name="authRepository"></param>
    public RefreshTokenQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public async Task<JwtTokens> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        if (! await _authRepository.CheckRefreshTokenRequest( request.RefreshToken))
        {
            throw new AuthenticationException("Invalid refresh token.");
        }
        var username = await _authRepository.GetUserNameByRefreshToken(request.RefreshToken);
        var stringToken = AccessTokenService.CreateAccessToken(username);
        return new JwtTokens { AccessToken = stringToken, RefreshToken = request.RefreshToken};
    }
}
