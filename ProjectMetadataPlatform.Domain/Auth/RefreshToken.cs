using System;
using Microsoft.AspNetCore.Identity;
namespace ProjectMetadataPlatform.Domain.Auth;

public class RefreshToken
{
    public string Token { get; set; }
    public IdentityUser User { get; set; }
    public DateTime ExpirationDate { get; set; }
}
