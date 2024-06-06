using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// DbContext for the project metadata platform database.
/// </summary>
public sealed class ProjectMetadataPlatformDbContext : DbContext
{
    /// <inheritdoc />
    public ProjectMetadataPlatformDbContext()
    {
       Database.Migrate(); 
    }
    
    /// <inheritdoc />
    public ProjectMetadataPlatformDbContext(DbContextOptions<ProjectMetadataPlatformDbContext> options) : base(options)
    {
        Database.Migrate();
    }
    
    /// <summary>
    /// Represents the table for project entities.
    /// </summary>
    public DbSet<Project> Projects { get; set; }
    
    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// Defines schema for the project table.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API surface for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableModel that defines the shape of your entities, the relationships between them, and how they map to the database.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            // Set the primary key for the Project entity
            entity.HasKey(e => e.Id);
                
            // Set the ProjectName property as required (non-nullable)
            entity.Property(e => e.ProjectName).IsRequired();
            
            // Set the ClientName property as required (non-nullable)
            entity.Property(e => e.ClientName).IsRequired();
            
            // Set the BusinessUnit property as required (non-nullable)
            entity.Property(e => e.BusinessUnit).IsRequired();
            
            // Set the TeamNumber property as required  (non-nullable)
            entity.Property(e => e.TeamNumber).IsRequired();
            
            // Set the Department property as required (non-nullable)
            entity.Property(e => e.Department).IsRequired();
        });
    }
}