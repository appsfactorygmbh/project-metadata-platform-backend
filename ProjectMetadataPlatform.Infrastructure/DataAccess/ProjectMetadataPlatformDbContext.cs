using Microsoft.EntityFrameworkCore;

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
}