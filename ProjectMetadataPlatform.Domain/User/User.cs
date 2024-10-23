using Microsoft.AspNetCore.Identity;
namespace ProjectMetadataPlatform.Domain.User;

public class User : IdentityUser
{
    private string Name { get; set; }
}
