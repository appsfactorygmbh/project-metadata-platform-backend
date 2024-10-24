namespace ProjectMetadataPlatform.Api.Users.Models;

public record CreateUserRequest(string UserId, string Username, string Name, string Email, string Password);
