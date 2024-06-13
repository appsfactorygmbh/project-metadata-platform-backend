using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// DbContext for the project metadata platform database.
/// </summary>
public sealed class ProjectMetadataPlatformDbContext : DbContext
{
    /// <summary>
    /// Represents the table for the relation between Project and Plugin entities.
    /// </summary>

    public DbSet<ProjectPlugins> ProjectPluginsRelation { get; set; }
    /// <summary>
    /// Represents the table for plugin entities.
    /// </summary>
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
        _ = Database.EnsureCreated();
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
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectMetadataPlatformDbContext).Assembly);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var project1 = new Project
        {
            Id = 100,
            ProjectName = "DB App",
            ClientName = "Deutsche Bahn",
            BusinessUnit = "Unit 1",
            TeamNumber = 1,
            Department = "Department 1"
        };

        var project2 = new Project
        {
            Id = 200,
            ProjectName = "Tagesschau App",
            ClientName = "ARD",
            BusinessUnit = "Unit 2",
            TeamNumber = 2,
            Department = "Department 2"
        };

        var project3 = new Project
        {
            Id = 300,
            ProjectName = "AOK Bonus App",
            ClientName = "AOK",
            BusinessUnit = "Unit 3",
            TeamNumber = 3,
            Department = "Department 3"
        };

        var plugin1 = new Plugin
        {
            Id = 100,
            PluginName = "Gitlab",
        };

        var plugin2 = new Plugin
        {
            Id = 200,
            PluginName = "SonarQube",
        };

        var plugin3 = new Plugin
        {
            Id = 300,
            PluginName = "Jira",
        };

        modelBuilder.Entity<Project>().HasData(project1, project2, project3);
        modelBuilder.Entity<Plugin>().HasData(plugin1, plugin2, plugin3);

        modelBuilder.Entity<ProjectPlugins>().HasData(
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/100",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/102",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/200",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/204",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/401",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/404",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/406",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/411",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/414",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            }
            );
    }
}
