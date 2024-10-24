namespace ProjectMetadataPlatform.Api.Users.Models;

public record PatchUserRequest(string? Username = null, string? Name = null, string? Email = null, string? Password = null);
