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

    public Report RunSimulation(Unit firstUnit, Unit secondUnit, int iterations)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(iterations);

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

                battleReport.T1TotalHits += attackData.NormalHits;
                battleReport.T1TotalCriticalHits += attackData.CriticalHits;
                battleReport.T1TotalAttacks += attackData.Attacks;
                battleReport.T1TotalDamageDealt += attackData.DamageDealt;
                battleReport.T2TotalDodgedAttacks += attackData.DodgedAttacks;
                battleReport.T2TotalArmorApplied += attackData.ArmorApplied;
                battleReport.T2TotalBlockedHits += attackData.BlockedHits;

                if (_ruleset.IsDefeated(team2))
                    break;
                
                //team2 Counterattacks
                var counterattackData = ResolveCombat(team2, team1);

                battleReport.T2TotalHits += counterattackData.NormalHits;
                battleReport.T2TotalCriticalHits += counterattackData.CriticalHits;
                battleReport.T2TotalAttacks += counterattackData.Attacks;
                battleReport.T2TotalDamageDealt += counterattackData.DamageDealt;
                battleReport.T1TotalDodgedAttacks += counterattackData.DodgedAttacks;
                battleReport.T1TotalArmorApplied += counterattackData.ArmorApplied;
                battleReport.T1TotalBlockedHits += counterattackData.BlockedHits;

                if (_ruleset.IsDefeated(team1))
                    break;
            }

            if (_ruleset.IsDefeated(team1) && _ruleset.IsDefeated(team2))
            {
                battleReport.Draws++;
            }
            else if (_ruleset.IsDefeated(team1))
            {
                battleReport.T2Wins++;
                battleReport.T2TotalRoundsToWin += turns;
                battleReport.T2TotalHealthLostOnWIn += (team2.MaxHealth - team2.CurrentHealth);
            }
            else if (_ruleset.IsDefeated(team2))
            {
                battleReport.T1Wins++;
                battleReport.T1TotalRoundsToWin += turns;
                battleReport.T1TotalHealthLostOnWIn += (team1.MaxHealth - team1.CurrentHealth);
            }
        }

        return battleReport;
    }
}