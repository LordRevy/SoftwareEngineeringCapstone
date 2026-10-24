using AUBE.Objects;

namespace AUBE.Mechanics;

public sealed class BattleSimulator
{
    private const int MaxTurns = 100;
    private readonly IRuleset _ruleset;

    public BattleSimulator(IRuleset ruleset)
    {
        _ruleset = ruleset;
    }

    // ===========================================================================================================
    //                                                                                            HELPER METHODS
    // ===========================================================================================================

    // --------------------------------------------------------------------------------- CREATE COMBATANT
    private static Unit CreateCombatant(Unit unit)
    {
        return new Unit
        {
            Id = unit.Id,
            Ruleset = unit.Ruleset,
            Type = unit.Type,
            Name = unit.Name,
            MaxHealth = unit.MaxHealth,
            CurrentHealth = unit.MaxHealth,
            Weapon = unit.Weapon,
            Armor = unit.Armor,
            AttackSkill = unit.AttackSkill,
            DodgeChance = unit.DodgeChance,
            BlockChance = unit.BlockChance,
            AbilityIds = [.. unit.AbilityIds]
        };
    }

    // --------------------------------------------------------------------------------- RESOLVE COMBAT
    private AttackResult ResolveCombat(Unit attacker, Unit defender)
    {
        
        var attack = _ruleset.ResolveAttack(attacker, defender);

        var hits = attack.NormalHits;
        var criticalHits = attack.CriticalHits;
        var attacks = attack.Attacks;
        var dodgedAttacks = attack.DodgedAttacks;

        var defense = _ruleset.ResolveDefense(attacker, defender, attack);

        var armorApplied = defense.ArmorApplied;
        var blockedHits = defense.BlockedHits;
        var damageDealt = defense.DamageDealt;

        return new AttackResult(
            NormalHits: hits,
            CriticalHits: criticalHits,
            Attacks: attacks,
            DodgedAttacks: dodgedAttacks,
            DamageDealt: damageDealt,
            ArmorApplied: armorApplied, 
            BlockedHits: blockedHits
            );
    }

    // ------------------------------------------------------------------------ RECORD ATTACK DATA
    private void AddAttackData(bool isTeam1, AttackResult data, Report report)
    {
        if (isTeam1)
        {
            report.T1TotalHits += data.NormalHits;
            report.T1TotalCriticalHits += data.CriticalHits;
            report.T1TotalAttacks += data.Attacks;
            report.T1TotalDamageDealt += data.DamageDealt;
        }
        else
        {
            report.T2TotalHits += data.NormalHits;
            report.T2TotalCriticalHits += data.CriticalHits;
            report.T2TotalAttacks += data.Attacks;
            report.T2TotalDamageDealt += data.DamageDealt;
        }
    }

    // ----------------------------------------------------------------------- RECORD DEFENSE DATA
    private void AddDefenseData(bool isTeam1, AttackResult data, Report report)
    {
        if (isTeam1)
        {
            report.T1TotalDodgedAttacks += data.DodgedAttacks;
            report.T1TotalArmorApplied += data.ArmorApplied;
            report.T1TotalBlockedHits += data.BlockedHits;
        }
        else
        {
            report.T2TotalDodgedAttacks += data.DodgedAttacks;
            report.T2TotalArmorApplied += data.ArmorApplied;
            report.T2TotalBlockedHits += data.BlockedHits;
        }
    }

    // ----------------------------------------------------------------------- RECORD WIN DATA
    private void RecordWinData(bool isTeam1, int turns, int healthLost, Report report)
    {
        if (isTeam1)
        {
            report.T1Wins++;
            report.T1TotalRoundsToWin += turns;
            report.T1TotalHealthLostOnWin += healthLost;
        }
        else
        {
            report.T2Wins++;
            report.T2TotalRoundsToWin += turns;
            report.T2TotalHealthLostOnWin += healthLost;
        }
    }

    // ===========================================================================================================
    //                                                                                           RUN SIMULATION
    // ===========================================================================================================

    public Report RunSimulation(Unit firstUnit, Unit secondUnit, int iterations)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(iterations);

        if (firstUnit.Id == secondUnit.Id)
            throw new ArgumentException("A unit cannot fight itself.");

        var battleReport = new Report{
            Id = 0,
            Iterations = iterations,
            T1Id = firstUnit.Id,
            T1Name = firstUnit.Name,
            T2Id = secondUnit.Id,
            T2Name = secondUnit.Name,
            CreatedAt = DateTime.Now
            };


        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var team1 = CreateCombatant(firstUnit);
            var team2 = CreateCombatant(secondUnit);
            var turns = 0;


            while (!_ruleset.IsDefeated(team1) && !_ruleset.IsDefeated(team2) && turns < MaxTurns)
            {
                turns++;

                //team1 Attacks
                var attackData = ResolveCombat(team1, team2);
                AddAttackData(isTeam1: true, attackData, battleReport);
                AddDefenseData(isTeam1: false, attackData, battleReport);

                if (_ruleset.IsDefeated(team2))
                    break;
                
                //team2 Counterattacks
                var counterattackData = ResolveCombat(team2, team1);
                AddAttackData(isTeam1: false, counterattackData, battleReport);
                AddDefenseData(isTeam1: true, counterattackData, battleReport);

                if (_ruleset.IsDefeated(team1))
                    break;
            }

            if (_ruleset.IsDefeated(team1) && _ruleset.IsDefeated(team2) || turns >= MaxTurns)
            {
                battleReport.Draws++;
            }
            else if (_ruleset.IsDefeated(team2))
            {
                var remainingHealth = team1.MaxHealth - team1.CurrentHealth;
                RecordWinData(isTeam1: true, turns, remainingHealth, battleReport);
            }
            else if (_ruleset.IsDefeated(team1))
            {
                var remainingHealth = team2.MaxHealth - team2.CurrentHealth;
                RecordWinData(isTeam1: false, turns, remainingHealth, battleReport);
            }
        }

        return battleReport;
    }
}