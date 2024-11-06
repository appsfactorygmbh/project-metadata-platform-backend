namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
///    Represents the request to create a new user.
/// </summary>
/// <param name="Username"> Username of a user</param>
/// <param name="Name">Name of a user</param>
/// <param name="Email">Email address of a user</param>
/// <param name="Password">Password of a user</param>
public record CreateUserRequest(string Username, string Name, string Email, string Password);
