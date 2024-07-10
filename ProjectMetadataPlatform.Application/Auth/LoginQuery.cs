using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for logging in.
/// </summary>
/// <param name="Username"></param>
/// <param name="Password"></param>
public record LoginQuery(string Username, string Password) : IRequest<JwtTokens>;

