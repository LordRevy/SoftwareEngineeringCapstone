using Microsoft.EntityFrameworkCore;
using AUBE.Objects;
using AUBE.Data;


namespace AUBE.TestScripts;
public static class TestDatabase
{
    public static void Run(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AubeDbContext>();

        context.Database.Migrate();

        var weapon = context.Weapons.FirstOrDefault(w => w.Id == 1000);

        if (weapon == null)
        {
            weapon = new Weapon
            {
                Id = 1000,
                Ruleset = RulesetType.D6,
                Type = WeaponType.Melee,
                Name = "Gladius",
                Damage = 1,
                NumberOfAttacks = 2,
                AttackModifier = 0,
                HitModifier = 0
            };

            context.Weapons.Add(weapon);
        }

        if (!context.Units.Any(unit => unit.Id == 1000))
        {
            context.Units.Add(new Unit
            {
                Id = 1000,
                Ruleset = RulesetType.D6,
                Type = "Heavy Infantry",
                Name = "Legionnaire",
                MaxHealth = 10,
                CurrentHealth = 10,
                Weapon = weapon,
                Armor = 2,
                AttackSkill = 3,
                DodgeChance = 5,
                BlockChance = 3,
                AbilityIds = new List<string> { "ShieldWall", "Charge" }
            });
        }

        context.SaveChanges();

        var savedUnit = context.Units.FirstOrDefault(unit => unit.Id == 1000);
        var savedWeapon = context.Weapons.FirstOrDefault(weapon => weapon.Id == 1000);

        Console.WriteLine($"Unit: {savedUnit?.Type}");
        Console.WriteLine($"Weapon: {savedWeapon?.Name} with Damage: {savedWeapon?.Damage}");
    }
}