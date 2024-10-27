using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the User.
/// </summary>
public class UserModelConfig : IEntityTypeConfiguration<User>
{

    /// <summary>
    /// Configures the User entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        _ = builder.HasKey(e => e.Id);
    }
}
