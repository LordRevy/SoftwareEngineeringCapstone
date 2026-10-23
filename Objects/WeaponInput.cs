using System.ComponentModel.DataAnnotations;
namespace AUBE.Objects;

/// <summary>
/// Represents the input model for adding a new weapon to the database.
/// </summary>
public sealed class AddWeaponInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(int.MinValue, int.MaxValue)]
    public int Damage { get; set; } = 1;

    [Range(int.MinValue, int.MaxValue)]
    public int NumberOfAttacks { get; set; } = 1;

    [Range(int.MinValue, int.MaxValue)]
    public int AttackModifier { get; set; } = 0;
    
    [Range(int.MinValue, int.MaxValue)]
    public int HitModifier { get; set; } = 0;
}