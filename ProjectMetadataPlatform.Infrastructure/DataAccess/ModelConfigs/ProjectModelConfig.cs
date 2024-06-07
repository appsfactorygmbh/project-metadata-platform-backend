using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Projects.
/// </summary>
public class ProjectModelConfig : IEntityTypeConfiguration<Project>
{
    
    /// <summary>
    /// Configures the Project entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Set the primary key for the Project entity
        _ = builder.HasKey(e => e.Id);

        // Set the ProjectName property as required (non-nullable)
        _ = builder.Property(e => e.ProjectName).IsRequired();

        // Set the ClientName property as required (non-nullable)
        _ = builder.Property(e => e.ClientName).IsRequired();

        // Set the BusinessUnit property as required (non-nullable)
        _ = builder.Property(e => e.BusinessUnit).IsRequired();
            
        // Set the TeamNumber property as required  (non-nullable)
        builder.Property(e => e.TeamNumber).IsRequired();

        // Set the Department property as required (non-nullable)
        _ = builder.Property(e => e.Department).IsRequired();

        _ = builder.HasMany(p => p.ProjectPlugins)
            .WithOne(pp => pp.Project)
            .HasForeignKey(pp => pp.ProjectId);
    }
}