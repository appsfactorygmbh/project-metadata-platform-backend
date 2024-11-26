namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
///     Represents a response for the GetAllUsers API call.
/// </summary>
/// <param name="Id">The id of the user.</param>
/// <param name="Email">The Email of the user.</param>
public record GetAllUsersResponse(
    string Id,
    string Email
    );
