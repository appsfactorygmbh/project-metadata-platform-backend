using MediatR;
using ProjectMetadataPlatform.Domain.Auth;


namespace ProjectMetadataPlatform.Application.Auth;

public record RefreshTokenQuery(string RefreshToken) : IRequest<JwtTokens>;
