using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for logging in.
/// </summary>
/// <param name="Username">Username of the user</param>
/// <param name="Password">Password of the user</param>
public record LoginQuery(string Username, string Password) : IRequest<JwtTokens>;

