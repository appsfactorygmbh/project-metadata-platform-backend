using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.User
{
    /// <summary>
    /// Represents a user in the system.
    /// Inherits from <see cref="IdentityUser"/> to include ASP.NET Core Identity functionality.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }
    }
}
