using AUBE.Objects;

namespace AUBE.Mechanics;

public sealed record AttackResult(
    int NormalHits = 0,
    int CriticalHits = 0,
    int Attacks = 0,
    int DodgedAttacks = 0,
    int BlockedHits = 0,
    int DamageDealt = 0,
    int ArmorApplied = 0
);

public interface IRuleset
{
    IReadOnlyDictionary<string, string> RuleExplanations { get; }

    AttackResult ResolveAttack(Unit attacker, Unit defender);

    AttackResult ResolveDefense(
        Unit attacker,
        Unit defender,
        AttackResult attackResult
    );

    bool IsDefeated(Unit unit);
}
