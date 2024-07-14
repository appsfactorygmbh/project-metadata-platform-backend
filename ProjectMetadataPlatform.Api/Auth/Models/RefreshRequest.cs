namespace ProjectMetadataPlatform.Api.Auth.Models;

public record RefreshRequest(string AccessToken, string RefreshToken);
