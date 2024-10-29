using System.Text;

namespace ProjectMetadataPlatform.Api.Users.Models;
/// <summary>
/// Represents the response model for retrieving user information.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Username">The username of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
public record GetUserResponse(string Id, string Username = "", string Name = "", string Email = "");
