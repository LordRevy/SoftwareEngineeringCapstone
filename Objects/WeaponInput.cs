using System.ComponentModel.DataAnnotations;
namespace AUBE.Objects;

/// <summary>
/// Represents the input model for adding a new weapon to the database.
/// </summary>
public sealed class AddWeaponInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Damage { get; set; } = 1;

    [Range(0, 100)]
    public int NumberOfAttacks { get; set; } = 1;

    [Range(-100, 100)]
    public int AttackModifier { get; set; } = 0;
    
    [Range(-100, 100)]
    public int HitModifier { get; set; } = 0;
}