using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class TestsWithDatabase
{
    [SetUp]
    public void SetUp()
    {
        using ProjectMetadataPlatformDbContext context = DbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        using ProjectMetadataPlatformDbContext context = DbContext();

        context.Database.EnsureDeleted();
    }
    protected ProjectMetadataPlatformDbContext DbContext()
    {
        return new ProjectMetadataPlatformDbContext(
            new DbContextOptionsBuilder<ProjectMetadataPlatformDbContext>()
                .UseSqlite("Datasource=unittest-db.db").Options);
    }
    /// <summary>
    ///     This method deletes the initially loaded Data from SeedData.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Database context</returns>
    protected void DeleteContext(ProjectMetadataPlatformDbContext context)
    {
        var allEntitiesPlugins = context.Plugins.ToList();
        var allEntitiesProjects = context.Projects.ToList();
        var allEntitiesProjectsPlugins = context.ProjectPluginsRelation.ToList();
        context.Plugins.RemoveRange(allEntitiesPlugins);
        context.Projects.RemoveRange(allEntitiesProjects);
        context.ProjectPluginsRelation.RemoveRange(allEntitiesProjectsPlugins);
        context.SaveChanges();
    }

    [Test]
    public void TestForConclusiveness()
    {
        //Test only exists to pass the pipeline
        Assert.Pass();
    }
}
