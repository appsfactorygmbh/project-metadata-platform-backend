namespace ProjectMetadataPlatform.Api.Auth.Models;

/// <summary>
///   Request for logging in.
/// </summary>
/// <param name="Username">Username of the User</param>
/// <param name="Password">Password of the User</param>
public record LoginRequest(string Username, string Password);
