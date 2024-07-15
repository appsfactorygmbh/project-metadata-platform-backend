using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class LogModelConfig : IEntityTypeConfiguration<Log>
{

    public void Configure(EntityTypeBuilder<Log> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasOne(l => l.Project)
            .WithMany(project => project.ProjectLogs)
            .HasForeignKey(p => p.ProjectId);

    }
}
