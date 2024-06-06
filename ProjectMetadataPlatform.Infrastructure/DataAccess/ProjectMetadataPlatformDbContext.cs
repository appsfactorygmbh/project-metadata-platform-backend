using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// DbContext for the project metadata platform database.
/// </summary>
public sealed class ProjectMetadataPlatformDbContext : DbContext
{
    
    public DbSet<ProjectPlugins> ProjectPluginsRelation { get; set; }
    public DbSet<Plugin> Plugins { get; set; }
    /// <summary>
    /// Represents the table for project entities.
    /// </summary>
    public DbSet<Project> Projects { get; set; }
    
    
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
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API surface for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableModel that defines the shape of your entities, the relationships between them, and how they map to the database.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectMetadataPlatformDbContext).Assembly);
    }
}