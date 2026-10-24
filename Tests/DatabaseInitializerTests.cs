using AUBE.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace AUBE.Tests;

public class DatabaseInitializerTests
{
    private readonly ITestOutputHelper _output;

    public DatabaseInitializerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ===========================================================================================================
    //                                                                                      DEVELOPMENT DATABASE
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- MIGRATE & SEED
    [Fact]
    public async Task InitializeAsync_MigratesAndSeedsDevelopmentDatabaseOnce()
    {
        _output.WriteLine(
            "======================================\n"+
            "Migrate & Seed Developmental Database\n" +
            "======================================");
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
        _output.WriteLine(
            "======================================\n"+
            "    Migrate Production Database\n" +
            "======================================");
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