using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// DbContext for the project metadata platform database.
/// </summary>
public sealed class ProjectMetadataPlatformDbContext : DbContext
{
    
    public DbSet<ProjectPlugins> ProjectPluginsRelation { get; set; }
    public DbSet<ProjectPlugins> Plugins { get; set; }

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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectMetadataPlatformDbContext).Assembly);
    }
}