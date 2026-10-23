namespace AUBE.Objects;

/// <summary>
/// Represents a weapon that a unit can be equipped with. 
/// Each weapon has a type (melee or ranged), a name, damage value, and an optional attack modifier.
/// </summary>
public class Weapon
{
    public required int Id              { get; set; }                   // Unique identifier for the weapon
    public required RulesetType Ruleset { get; set; }                   // The Ruleset that the Weapon belongs to
    public required WeaponType Type     { get; set; }                   // Type of weapon (Melee or Ranged)
    public required string Name         { get; set; }                   // Name of the weapon
    public required int Damage          { get; set; }                   // Damage dealt by the weapon
    public int NumberOfAttacks          { get; set; } = 1;              // Number of attacks the weapon can perform per use
    public int AttackModifier           { get; set; } = 0;              // Modifier to the unit's attack skill when using this weapon
    public int HitModifier              { get; set; } = 0;              // Modifier to the unit's hit chance when using this weapon
}