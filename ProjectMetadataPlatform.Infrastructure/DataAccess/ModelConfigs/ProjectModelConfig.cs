using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Configuration class for the Project entity model.
/// </summary>
public class ProjectnModelConfig : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configures the entity of type Project.
    /// </summary>
    /// <param name="builder">Provides a simple API surface for configuring a Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder that defines the shape of your entities, the relationships between them, and how they map to the database.</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Sets the primary key for the Project entity to be the Id property.
        builder.HasKey(e => e.Id);
    }
}