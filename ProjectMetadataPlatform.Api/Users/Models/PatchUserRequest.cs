namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents a request model for patching user information.
/// </summary>
/// <param name="Username">The new username of the user, or null to leave unchanged.</param>
/// <param name="Name">The new name of the user, or null to leave unchanged.</param>
/// <param name="Email">The new email address of the user, or null to leave unchanged.</param>
/// <param name="Password">The new password of the user, or null to leave unchanged.</param>
public record PatchUserRequest(string? Username = null, string? Name = null, string? Email = null, string? Password = null);
