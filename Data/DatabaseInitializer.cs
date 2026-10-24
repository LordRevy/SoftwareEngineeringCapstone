using AUBE.Objects;
using Microsoft.EntityFrameworkCore;

namespace AUBE.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        AubeDbContext dbContext,
        bool seedDemoData,
        CancellationToken cancellationToken = default)
    {
        await ApplyMigrationsAsync(dbContext, cancellationToken);

        if (seedDemoData)
        {
            await SeedDemoDataAsync(dbContext, cancellationToken);
        }
    }

    // ===========================================================================================================
    //                                                                                   DATABASE INITIALIZATION
    // ===========================================================================================================
    
    // ------------------------------------------------ APPLY DATABASE MIGRATIONS / CREATE DATABASE FOLDER
    private static async Task ApplyMigrationsAsync(AubeDbContext dbContext, CancellationToken cancellationToken)
    {
        var databasePath = dbContext.Database.GetDbConnection().DataSource;

        if (!string.IsNullOrWhiteSpace(databasePath) && databasePath != ":memory:")
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(databasePath));

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
        }

        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    // ===========================================================================================================
    //                                                                                              SAMPLE DATA
    // ===========================================================================================================

    // ------------------------------------------------------------------- CREATE SAMPLE DATA IF NONE EXIST
    private static async Task SeedDemoDataAsync(AubeDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Units.AnyAsync(cancellationToken))
            return;

        var weapon = CreateDemoWeapon();

        dbContext.Weapons.Add(weapon);

        dbContext.Units.AddRange(
            CreateDemoUnit("Training Guard", weapon, armor: 1),
            CreateDemoUnit("Training Raider", weapon, armor: 0));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    // ----------------------------------------------------------------------------------- CREATE DEMO WEAPON
    private static Weapon CreateDemoWeapon()
    {
        return new Weapon
        {
            Id = 0,
            Ruleset = RulesetType.D6,
            Type = WeaponType.Melee,
            Name = "Training Blade",
            Damage = 1,
            NumberOfAttacks = 1
        };
    }
    
    // ----------------------------------------------------------------------------------- CREATE DEMO UNIT
    private static Unit CreateDemoUnit(string name, Weapon weapon, int armor)
    {
        return new Unit
        {
            Id = 0,
            Ruleset = RulesetType.D6,
            Type = "Infantry",
            Name = name,
            MaxHealth = 5,
            CurrentHealth = 5,
            Weapon = weapon,
            Armor = armor,
            AttackSkill = 4,
            AbilityIds = []
        };
    }
}