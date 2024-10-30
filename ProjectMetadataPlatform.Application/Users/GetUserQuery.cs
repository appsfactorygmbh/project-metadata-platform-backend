using MediatR;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
///     Query to get a user by id.
/// </summary>
public record GetUserQuery(string UserId) : IRequest<User?>;
