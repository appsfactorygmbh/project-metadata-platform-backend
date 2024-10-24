namespace ProjectMetadataPlatform.Api.Users.Models;

public record CreateUserRequest(int UserId, string Username, string Name, string Email, string Password);
