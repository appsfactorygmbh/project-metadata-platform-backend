using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class TestsWithDatabase
{
    protected ProjectMetadataPlatformDbContext DbContext() => new(
        new DbContextOptionsBuilder<ProjectMetadataPlatformDbContext>()
            .UseSqlite("Datasource=test-db.db").Options);

    [SetUp]
    public void SetUp()
    {
        using var context = DbContext();

        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    [TearDown]
    public void TearDown()
    {
        using var context = DbContext();

        context.Database.EnsureDeleted();
    }
}