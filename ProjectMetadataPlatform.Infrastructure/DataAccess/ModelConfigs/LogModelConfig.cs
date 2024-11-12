using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
///     Configuration of the Log Table in the database.
/// </summary>
public class LogModelConfig : IEntityTypeConfiguration<Log>
{

    /// <summary>
    ///     Configures Log entity
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder
            .HasOne(e => e.Project)
            .WithMany(e => e.Logs)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired();

        _ = builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
