using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
///     Handler for the <see cref="LoginQuery" />
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
        var stringToken = AccessTokenService.CreateAccessToken(request.Username);
        var refreshToken = Guid.NewGuid().ToString();
        if (await _authRepository.CheckRefreshTokenExists(request.Username))
        {
            await _authRepository.UpdateRefreshToken(request.Username, refreshToken);
        }
        else
        {
            await _authRepository.StoreRefreshToken(request.Username, refreshToken);
        }
        return new JwtTokens { AccessToken = stringToken, RefreshToken = refreshToken };
    }
}
