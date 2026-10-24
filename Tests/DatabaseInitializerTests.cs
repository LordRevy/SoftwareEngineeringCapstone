using AUBE.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AUBE.Tests;

public class DatabaseInitializerTests
{
    // ===========================================================================================================
    //                                                                                      DEVELOPMENT DATABASE
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- MIGRATE & SEED
    [Fact]
    public async Task InitializeAsync_MigratesAndSeedsDevelopmentDatabaseOnce()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AubeDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AubeDbContext(options);

        await DatabaseInitializer.InitializeAsync(dbContext, seedDemoData: true);
        await DatabaseInitializer.InitializeAsync(dbContext, seedDemoData: true);

        Assert.True(await dbContext.Database.CanConnectAsync());
        Assert.Equal(2, await dbContext.Units.CountAsync());
        Assert.Single(await dbContext.Weapons.ToListAsync());
    }

    // ===========================================================================================================
    //                                                                                       PRODUCTION DATABASE
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- MIGRATE WITHOUT SEED
    [Fact]
    public async Task InitializeAsync_LeavesProductionDatabaseWithoutDemoRows()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AubeDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AubeDbContext(options);

        await DatabaseInitializer.InitializeAsync(dbContext, seedDemoData: false);

        Assert.True(await dbContext.Database.CanConnectAsync());
        Assert.Empty(await dbContext.Units.ToListAsync());
        Assert.Empty(await dbContext.Weapons.ToListAsync());
    }
}