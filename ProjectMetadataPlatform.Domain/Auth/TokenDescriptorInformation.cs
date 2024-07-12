namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
///    Information about the token descriptor.
/// </summary>
public class TokenDescriptorInformation
{
    /// <summary>
    ///    The valid issuer.
    /// </summary>
    public string? ValidIssuer { get; set; }
    /// <summary>
    ///   The valid audience.
    /// </summary>
    public string? ValidAudience { get; set; }
    /// <summary>
    ///   The issuer signing key.
    /// </summary>
    public string? IssuerSigningKey { get; set; }
}
