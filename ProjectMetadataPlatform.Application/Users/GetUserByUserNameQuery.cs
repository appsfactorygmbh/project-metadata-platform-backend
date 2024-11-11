using MediatR;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a query to get a user by their username.
/// </summary>
/// <param name="UserName">The username of the user to retrieve.</param>
/// <returns>The user with the specified username, or null if not found.</returns>
public record GetUserByUserNameQuery(string UserName) : IRequest<User?>;
