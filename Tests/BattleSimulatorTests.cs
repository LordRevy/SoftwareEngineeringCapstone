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
    //                                                                                               SIMULATIONS
    // ===========================================================================================================

    // ---------------------------------------------------------------------- RECORD WINS AND DOESNT CHANGE UNIT
    [Fact]
    public void RunSimulation_RecordsWinsAndDoesNotChangeSavedUnits()
    {
        _output.WriteLine(
            "======================================\n"+
            "Record Wins And Ensure Unit Integrity\n" +
            "======================================");
        // Uses fixed combat outcomes so this test does not depend on random dice rolls.
        var ruleset = new AlwaysDefeatsOnHitRuleset();
        var simulator = new BattleSimulator(ruleset);
        var first = CreateUnit(1, "First", health: 3);
        var second = CreateUnit(2, "Second", health: 3);

        var report = simulator.RunSimulation(first, second, iterations: 4);

        Assert.Equal(4, report.Iterations);
        Assert.Equal(4, report.T1Wins);
        Assert.Equal(0, report.T2Wins);
        Assert.Equal(0, report.Draws);
        Assert.Equal(12, report.T1TotalDamageDealt);
        Assert.Equal(3, first.CurrentHealth);
        Assert.Equal(3, second.CurrentHealth);
    }

    // ------------------------------------------------------------------------------ VALIDATE ITERATION COUNT
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RunSimulation_RejectsNonPositiveIterationCounts(int iterations)
    {
        _output.WriteLine(
            "===================\n"+
            "   Iteration Test\n" +
            "===================");
        var simulator = new BattleSimulator(new AlwaysDefeatsOnHitRuleset());

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            simulator.RunSimulation(
                CreateUnit(1, "First", health: 1),
                CreateUnit(2, "Second", health: 1),
                iterations));
    }

    [Fact]
    public void D6Ruleset_ZeroDodgeAndBlockChanceDoNotAutomaticallySucceed()
    {
        _output.WriteLine(
            "================================================\n"+
            "Dodge and Block of 0 Dont Automatically Succeed\n" +
            "================================================");
        var ruleset = new D6Ruleset();
        var attacker = CreateUnit(1, "Attacker", health: 3);
        var defender = CreateUnit(2, "Defender", health: 3);

        var attack = ruleset.ResolveAttack(attacker, defender);
        var defense = ruleset.ResolveDefense(
            attacker,
            defender,
            new AttackResult(NormalHits: 1));

        Assert.Equal(0, attack.DodgedAttacks);
        Assert.Equal(1, defense.DamageDealt);
        Assert.Equal(0, defense.BlockedHits);
        Assert.Equal(2, defender.CurrentHealth);
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