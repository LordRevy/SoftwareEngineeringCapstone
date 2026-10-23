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

    [Range(int.MinValue, int.MaxValue)]
    public int Health { get; set; } = 1;

    [Range(int.MinValue, int.MaxValue)]
    public int WeaponId { get; set; }

    [Range(int.MinValue, int.MaxValue)]
    public int Armor { get; set; } = 0;

    [Range(int.MinValue, int.MaxValue)]
    public int AttackSkill { get; set; } = 1;

    [Range(int.MinValue, int.MaxValue)]
    public int DodgeChance { get; set; } = 0;

    [Range(int.MinValue, int.MaxValue)]
    public int BlockChance { get; set; } = 0;

    public string? AbilityIds { get; set; }
}