using AUBE.Mechanics;
using AUBE.Objects;
using Xunit;
using Xunit.Abstractions;

namespace AUBE.Tests;

public class BattleSimulatorTests
{
    private readonly ITestOutputHelper _output;

    public BattleSimulatorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ===========================================================================================================
    //                                                                                           RECORD WIN TEST
    // ===========================================================================================================

    [Fact]
    public void RunSimulation_RecordsWinsAndDoesNotChangeSavedUnits()
    {
        _output.WriteLine(
            "======================================\n"+
            "Record Wins And Ensure Unit Integrity\n" +
            "======================================");
        
        var ruleset = new AlwaysDefeatsOnHitRuleset();
        _output.WriteLine("Ruleset: Always Defeats On Hit (Test Ruleset)");
        var simulator = new BattleSimulator(ruleset);
        _output.WriteLine("Creating BattleSimulator...");
        var first = CreateUnit(1, "First", health: 3);
        _output.WriteLine(
            $"Attacker: {first.Name}, health={first.CurrentHealth}, " +
            $"dodge={first.DodgeChance}, block={first.BlockChance}, " +
            $"weapon damage={first.Weapon.Damage}");
        var second = CreateUnit(2, "Second", health: 3);
        _output.WriteLine(
            $"Defender: {second.Name}, health={second.CurrentHealth}, " +
            $"armor={second.Armor}, dodge={second.DodgeChance}, " +
            $"block={second.BlockChance}");

        var report = simulator.RunSimulation(first, second, iterations: 4);

        _output.WriteLine("Checking Iterations == 4...");
        Assert.Equal(4, report.Iterations);
        _output.WriteLine("Checking First's Wins is == 4...");
        Assert.Equal(4, report.T1Wins);
        _output.WriteLine("Checking Second's Wins is == 0...");
        Assert.Equal(0, report.T2Wins);
        _output.WriteLine("Checking Draws is == 0...");
        Assert.Equal(0, report.Draws);
        _output.WriteLine("Checking First's Total Damage Dealt is == 12...");
        Assert.Equal(12, report.T1TotalDamageDealt);
        _output.WriteLine("Checking First's Current Health is == 3...");
        Assert.Equal(3, first.CurrentHealth);
        _output.WriteLine("Checking Second's Current Health is == 3...");
        Assert.Equal(3, second.CurrentHealth);

        _output.WriteLine("Test Succeeded");
    }

    // ===========================================================================================================
    //                                                                                      ITERATION COUNT TEST
    // ===========================================================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RunSimulation_RejectsNonPositiveIterationCounts(int iterations)
    {
        _output.WriteLine(
            "===================\n"+
            "   Iteration Test\n" +
            "===================");
        _output.WriteLine("Creating BattleSimulator | Ruleset: Always Defeats On Hit (Test Ruleset)");
        var simulator = new BattleSimulator(new AlwaysDefeatsOnHitRuleset());

        _output.WriteLine($"Ensuring Out Of Range Exception is thrown with {iterations} Iterations.");
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            simulator.RunSimulation(
                CreateUnit(1, "First", health: 1),
                CreateUnit(2, "Second", health: 1),
                iterations));
        _output.WriteLine("Test Succeeded.");
    }

    // ===========================================================================================================
    //                                                                                      DODGE AND BLOCK TEST
    // ===========================================================================================================

    [Fact]
    public void D6Ruleset_ZeroDodgeAndBlockChanceDoNotAutomaticallySucceed()
    {
        _output.WriteLine(
            "================================================\n"+
            "Dodge and Block of 0 Dont Automatically Succeed\n" +
            "================================================");
        var ruleset = new D6Ruleset();
        _output.WriteLine($"Ruleset: {ruleset.GetType().Name}");

        // ------------------------------------------------------------------------------ CREATE UNITS
        var attacker = CreateUnit(1, "Attacker", health: 3);
        _output.WriteLine(
            $"Attacker: {attacker.Name}, health={attacker.CurrentHealth}, " +
            $"dodge={attacker.DodgeChance}, block={attacker.BlockChance}, " +
            $"weapon damage={attacker.Weapon.Damage}");
        var defender = CreateUnit(2, "Defender", health: 3);
        _output.WriteLine(
            $"Defender: {defender.Name}, health={defender.CurrentHealth}, " +
            $"armor={defender.Armor}, dodge={defender.DodgeChance}, " +
            $"block={defender.BlockChance}");

        // ----------------------------------------------------------- CALL ATTACK AND DEFENSE METHODS
        var attack = ruleset.ResolveAttack(attacker, defender);
        _output.WriteLine($"Attack Result:\n{attack}");

        var defenseInput = new AttackResult(NormalHits: 1);
        _output.WriteLine($"Defense input: {defenseInput}");

        var defense = ruleset.ResolveDefense(attacker, defender, defenseInput);
        _output.WriteLine($"Defense Result:\n{defense}");
        _output.WriteLine($"Defender health after defense: {defender.CurrentHealth}");

        // ------------------------------------------------------------------- ASSERT STATEMENTS
        _output.WriteLine("Checking Dodged Attacks == 0...");
        Assert.Equal(0, attack.DodgedAttacks);
        _output.WriteLine("Checking Damage Dealt == 1...");
        Assert.Equal(1, defense.DamageDealt);
        _output.WriteLine("Checking Blocked Hits == 0...");
        Assert.Equal(0, defense.BlockedHits);
        _output.WriteLine("Checking Current Health == 2...");
        Assert.Equal(2, defender.CurrentHealth);
        
        _output.WriteLine("Test Succeeded");
    }

    // ===========================================================================================================
    //                                                                                               TEST HELPERS
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- CREATE TEST UNIT
    private static Unit CreateUnit(int id, string name, int health)
    {
        return new Unit
        {
            Id = id,
            Ruleset = RulesetType.D6,
            Type = "Test",
            Name = name,
            MaxHealth = health,
            CurrentHealth = health,
            Weapon = new Weapon
            {
                Id = id,
                Ruleset = RulesetType.D6,
                Type = WeaponType.Melee,
                Name = "Test Blade",
                Damage = 1
            }
        };
    }

    // ----------------------------------------------------------------------------------- DETERMINISTIC RULESET
    // Guarantees the first combatant wins immediately, making report assertions repeatable.
    private sealed class AlwaysDefeatsOnHitRuleset : IRuleset
    {
        public IReadOnlyDictionary<string, string> RuleExplanations { get; } =
            new Dictionary<string, string>();

        public AttackResult ResolveAttack(Unit attacker, Unit defender)
        {
            return new AttackResult(NormalHits: 1, Attacks: 1);
        }

        public AttackResult ResolveDefense(Unit attacker, Unit defender, AttackResult attackResult)
        {
            defender.CurrentHealth = 0;
            return new AttackResult(DamageDealt: 3);
        }

        public bool IsDefeated(Unit unit)
        {
            return unit.CurrentHealth <= 0;
        }
    }
}