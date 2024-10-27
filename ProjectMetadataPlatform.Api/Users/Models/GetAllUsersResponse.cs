namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
///     Represents a response for the GetAllUsers API call.
/// </summary>
/// <param name="Id">The id of the user.</param>
/// <param name="Name">The name of the user.</param>
public record GetAllUsersResponse(
    int Id,
    string Name);
