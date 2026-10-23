namespace AUBE.Objects;

/// <summary>
/// Represents a unit in the simulation. This can be a single individual or a group of individuals, depending on the context. 
/// Each unit has a unique identifier, a name, a type, health points, and is equipped with a weapon. 
/// Additionally, units can have armor, dodge chance, block chance, and can perform multiple attacks per turn.
/// Units can also have a list of abilities that they can use, which are derived from the ruleset of the game.
/// </summary>
public class Unit
{
    public required int Id              { get; set; }                       // Unique identifier for the unit
    public required RulesetType Ruleset { get; set; }                       // The Ruleset that this Unit belongs to
    public required string Type         { get; set; }                       // Type of unit (Legionnaires, Space Marine, etc.)
    public required string Name         { get; set; }                       // Name of the unit
    public required int MaxHealth       { get; set; }                       // Total health of the unit
    public required Weapon Weapon       { get; set; }                       // Weapon object that the unit is equipped with
    public int CurrentHealth            { get; set; }                       // Current health of the unit, can be less than MaxHealth
    public int Armor                    { get; set; } = 0;                  // Reduces damage taken from a hit
    public int AttackSkill              { get; set; } = 1;                  // Represents the unit's skill in attacking, which affects hit chance
    public int DodgeChance              { get; set; } = 0;                  // Chance to dodge an entire attack, value is dependent on Ruleset.
    public int BlockChance              { get; set; } = 0;                  // Chance to block/parry a single hit, value is dependent on Ruleset.
    public List<string> AbilityIds      { get; set; } = new List<string>(); // List of abilities. Derived from the chosen ruleset.
}
