using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class LogModelConfig : IEntityTypeConfiguration<Log>
{

    public void Configure(EntityTypeBuilder<Log> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder
            .HasOne(e => e.Project)
            .WithMany(e => e.Logs)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired();
    }
}
