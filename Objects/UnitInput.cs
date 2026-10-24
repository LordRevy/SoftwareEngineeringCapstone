using System.ComponentModel.DataAnnotations;
namespace AUBE.Objects;

/// <summary>
/// Represents the input model for adding a new unit to the database.
/// </summary>
public sealed class AddUnitInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int Health { get; set; } = 1;

    [Range(1, 10000)]
    public int WeaponId { get; set; }

    [Range(0, 100)]
    public int Armor { get; set; } = 0;

    [Range(0, 100)]
    public int AttackSkill { get; set; } = 1;

    [Range(0, 100)]
    public int DodgeChance { get; set; } = 0;

    [Range(0, 100)]
    public int BlockChance { get; set; } = 0;

    [StringLength(50)]
    public string? AbilityIds { get; set; }
}