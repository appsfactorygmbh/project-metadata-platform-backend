using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class UserModelConfig : IEntityTypeConfiguration<User>
{

    public void Configure(EntityTypeBuilder<User> builder)
    {
        _ = builder.HasKey(e => e.Id);
    }
}
