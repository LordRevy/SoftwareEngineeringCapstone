using AUBE.Objects;

namespace AUBE.Mechanics;

/// <summary>
/// Represents a D6 ruleset for the simulation.
/// D6 ruleset uses a six-sided die to determine the outcome of attacks, hits, and damage.
/// Typically, D6's are used when many models are involved in a battle, and the ruleset is designed to be simple and fast to resolve.
/// The D6 ruleset is based on the following principles:
/// - Each unit has a number of attacks based on their health (health representing the amount of warriors in the unit) and weapon's number of attacks
/// - The chance to hit is determined by the attacker's skill and weapon's hit modifier
/// - Damage is calculated based on the weapon's damage and the defender's armor
/// - Critical hits increase damage and reduce the defender's armor for that hit
/// - The defender will have a chance to dodge or block the attack based on their dodge and block chances
/// - A unit is considered defeated when its current health reaches zero or below
/// </summary>
public class D6Ruleset : IRuleset
{
    public IReadOnlyDictionary<string, string> RuleExplanations { get; } =
        new Dictionary<string, string>
    {
        ["Armor"] = "Reduces incoming weapon damage by 1 per point of Armor.",
        ["AttackSkill"] = "The D6 threshold needed to hit. A 4 means a roll of 4-6 hits.",
        ["DodgeChance"] = "Chance to dodge an entire attack. A 4 means a roll of 4-6 dodges.",
        ["BlockChance"] = "Chance to block each successful hit, resolved independently. A 4 means a roll of 4-6 blocks a hit.",
        ["AbilityIds"] = "Unique abilities possessed by the unit.",
        ["Damage"] = "Damage dealt by one successful hit before Armor is subtracted.",
        ["NumberOfAttacks"] = "Number of attacks a Weapon makes per warrior in a Unit.",
        ["AttackModifier"] = "Modifies the number of attacks generated per point of CurrentHealth.",
        ["HitModifier"] = "Modifies the hit threshold. A +1 modifier improves the threshold by one (4+ becomes 3+)"
    };

    private enum HitResult
    {
        ArmorNegated,
        GuardBlocked,
        WoundInflicted,
        NoneIdentified
    }

    // ===========================================================================================================
    //                                                                                             HELPER METHOD
    // ===========================================================================================================

    /// <summary>
    /// Resolves a single hit against a defending unit.
    /// Calculates damage after Armor, applies Deathblow bonuses if applicable,
    /// and determines the Guard threshold based on the remaining damage.
    /// If the damage is greater than zero, the defender rolls Guard.
    /// A failed Guard roll reduces the defender's CurrentHealth by 1.
    /// </summary>
    /// <param name="attacker">The unit that dealt the hit.</param>
    /// <param name="defender">The unit receiving the hit.</param>
    /// <param name="isDeathblow">Whether the hit is a Deathblow, granting +1 damage and reducing Armor by 1.</param>
    private HitResult ResolveHit(Unit attacker, Unit defender, bool isDeathblow)
    {
        var damage = attacker.Weapon.Damage;
        var armor = defender.Armor;

        if (isDeathblow)
        {
            damage += 1;
            armor = Math.Max(0, armor - 1);
        }

        var remainingDamage = damage - armor;

        if (remainingDamage <= 0)
            return HitResult.ArmorNegated;

        if (defender.BlockChance > 0)
        {
            var guard = Math.Clamp(defender.BlockChance + remainingDamage - 1, 1, 6);

            if (DiceHandler.RollDie(6, guard))
                return HitResult.GuardBlocked;
        }

        defender.CurrentHealth--;
        return HitResult.WoundInflicted;
    }

    // ===========================================================================================================
    //                                                                                            RESOLVE ATTACK
    // ===========================================================================================================

    /// <summary>
    /// Calculates and returns the amount of successful hits a Unit makes.
    /// Determines the amount of attacks based on the Units Weapons and CurrentHealth (Amount of warriors).
    /// Determines the threshold needed to hit and calls the RollDice method to determine successes.
    /// Creates an AttackResult with the resulting successes.
    /// </summary>
    /// <param name="attacker">The unit that dealt the hit.</param>
    /// <param name="defender">The unit receiving the hit.</param>
    /// <returns>AttackResult with NormalHits and CriticalHits (Deathblows) separated.</returns>
    public AttackResult ResolveAttack(Unit attacker, Unit defender)
    {
        var attacksPerWarrior = Math.Max(0, attacker.Weapon.NumberOfAttacks + attacker.Weapon.AttackModifier);
        var totalAttacks = attacksPerWarrior * attacker.CurrentHealth;

        var (dodgedAttacks, _) = defender.DodgeChance > 0
            ? DiceHandler.RollDice(6, totalAttacks, defender.DodgeChance)
            : (0, 0);
        var remainingAttacks = totalAttacks - dodgedAttacks;

        var hitThreshold = Math.Clamp(attacker.AttackSkill - attacker.Weapon.HitModifier, 2, 6);

        var (hits, deathblows) = DiceHandler.RollDice(6, remainingAttacks, hitThreshold, 6);
        
        return new AttackResult(
            NormalHits:hits,
            CriticalHits:deathblows,
            Attacks:totalAttacks,
            DodgedAttacks:dodgedAttacks
            );
    }

    // ===========================================================================================================
    //                                                                                           RESOLVE DEFENSE
    // ===========================================================================================================

    /// <summary>
    /// Resolves the incoming hits and deathblows from an attack.
    /// Calls the ResolveHit helper method for each strike.
    /// Handles deathblows by passing True to ResolveHit.
    /// </summary>
    /// <param name="attacker">The unit that dealt the hit.</param>
    /// <param name="defender">The unit receiving the hit.</param>
    /// <param name="attackResult">the amount of normal hits and deathblows from the incoming attack.</param>
    public AttackResult ResolveDefense(Unit attacker, Unit defender, AttackResult attackResult)
    {
        HitResult hitResult;
        
        var normalDamage = 0;
        var criticalDamage = 0;
        var armorBlocked = 0;
        var guardBlocked = 0;

        for (int i = 0; i < attackResult.NormalHits; i++)
        {
            hitResult = ResolveHit(attacker, defender, false);
            if (hitResult == HitResult.WoundInflicted)
                normalDamage++;
            else if (hitResult == HitResult.ArmorNegated)
                armorBlocked++;
            else if (hitResult == HitResult.GuardBlocked)
                guardBlocked++;
        }

        for (int i = 0; i < attackResult.CriticalHits; i++)
        {
            hitResult = ResolveHit(attacker, defender, true);
            if (hitResult == HitResult.WoundInflicted)
                criticalDamage++;
            else if (hitResult == HitResult.ArmorNegated)
                armorBlocked++;
            else if (hitResult == HitResult.GuardBlocked)
                guardBlocked++;
        }

        return new AttackResult(
            NormalHits:normalDamage,
            CriticalHits:criticalDamage,
            DamageDealt:normalDamage + criticalDamage,
            ArmorApplied:armorBlocked,
            BlockedHits:guardBlocked
            );
    }

    // ===========================================================================================================
    //                                                                                           CHECK FOR DEATH
    // ===========================================================================================================

    /// <summary>
    /// Checks if the Unit has health remaining.
    /// </summary>
    /// <param name="unit">The unit being checked for remaining health.</param>
    public bool IsDefeated(Unit unit)
    {
        return unit.CurrentHealth <= 0;
    }
}