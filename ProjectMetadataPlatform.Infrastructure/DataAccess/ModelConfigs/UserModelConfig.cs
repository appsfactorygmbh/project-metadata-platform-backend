using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class UserModelConfig : IEntityTypeConfiguration<IdentityUser>
{

    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        _ = builder.HasKey(e => e.Id);
    }
}
