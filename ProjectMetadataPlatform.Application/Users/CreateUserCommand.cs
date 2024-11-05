using MediatR;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
///     Command to create a new user.
/// </summary>
/// <param name="Username">Username of the user.</param>
/// <param name="Name">Name of the user.</param>
/// <param name="Email">Email of the user.</param>
/// <param name="Password">Password of the user.</param>
public record CreateUserCommand(string Username, string Name, string Email, string Password) : IRequest<string>;
