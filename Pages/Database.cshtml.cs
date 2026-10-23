using AUBE.Data;
using AUBE.Objects;
using AUBE.Mechanics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AUBE.Pages;

public class DatabaseModel : PageModel
{
    // ===========================================================================================================
    //                                                                                  CONSTRUCTOR & PROPERTIES
    // ===========================================================================================================

    private readonly AubeDbContext _dbContext;
    private readonly IRuleset _ruleset;

    public DatabaseModel(AubeDbContext dbContext, IRuleset ruleset)
    {
        _dbContext = dbContext;
        _ruleset = ruleset;
    }

    public List<Unit> Units { get; private set; } = [];
    public List<Weapon> Weapons { get; private set; } = [];
    public bool ShowUnitDialog { get; private set; }
    public bool ShowWeaponDialog { get; private set; }

    public async Task OnGetAsync(RulesetType? selectedRuleset)
    {
        if (selectedRuleset.HasValue &&
            Enum.IsDefined(selectedRuleset.Value))
        {
            SelectedRuleset = selectedRuleset.Value;
        }

        await LoadPageDataAsync();
    }

    public async Task<IActionResult> OnPostSelectedRulesetAsync()
    {
        await LoadPageDataAsync();

        return RedirectToPage(new
        {
            SelectedRuleset
        });
    }
    private async Task LoadPageDataAsync()
    {
        Units = await _dbContext.Units
            .AsNoTracking()
            .Include(unit => unit.Weapon)
            .OrderBy(unit => unit.Id)
            .ToListAsync();

        Weapons = await _dbContext.Weapons
            .AsNoTracking()
            .OrderBy(weapon => weapon.Id)
            .ToListAsync();
    }

    [BindProperty]
    public AddUnitInput UnitInput { get; set; } = new();
    
    [BindProperty]
    public AddWeaponInput WeaponInput { get; set; } = new();

    [BindProperty]
    public int SelectedUnitId { get; set; }
    
    [BindProperty]
    public int SelectedWeaponId { get; set; }

    [BindProperty(SupportsGet = true)]
    public RulesetType? SelectedRuleset { get; set; }

    [BindProperty]
    public WeaponType? SelectedWeapon { get; set; }

    public IReadOnlyDictionary<string, string> RuleExplanations
    {
        get
        {
            return SelectedRuleset switch
            {
                RulesetType.D6 => _ruleset.RuleExplanations,
                _ => new Dictionary<string, string>()
            };
        }
    }

    // ===========================================================================================================
    //                                                                                            HELPER METHODS
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- VALIDATE MODEL
    /// <summary>
    /// Checks UnitInput and WeaponInput for validity by verifying field values.
    /// </summary>
    /// <param name="prefix">The prefix for the properties to check.</param>
    /// <returns>true if the input is valid; otherwise, false.</returns>
    private bool IsInputValid(string prefix)
    {
        var prefixWithSeparator = prefix + ".";

        var entries = ModelState
            .Where(entry => entry.Key.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                entry.Key.StartsWith(prefixWithSeparator, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return entries.Count > 0 && entries.All(entry => entry.Value?.ValidationState ==
            Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid);
    }

    // ----------------------------------------------------------------------------------- GET WEAPON
    /// <summary>
    /// Takes the user input and returns a matching WeaponType, or Null if not found. 
    /// </summary>
    private WeaponType? GetWeaponType()
    {
        if (SelectedWeapon.HasValue && Enum.IsDefined(SelectedWeapon.Value))
            return SelectedWeapon.Value;
        
        return null;
    }

    // ----------------------------------------------------------------------------------- GET RULESET
    /// <summary>
    /// Grabs the chosen Ruleset from the Page Header to ensure its in the correct catagory.
    /// </summary>
    private RulesetType? GetRuleset()
    {
        if (SelectedRuleset.HasValue && Enum.IsDefined(SelectedRuleset.Value))
            return SelectedRuleset;

        return null;
    }

    // ----------------------------------------------------------------------------------- ADD UNIT
    /// <summary>
    /// Creates a new Unit object from the provided Input and associated Weapon.
    /// </summary>
    /// <param name="weapon">The Weapon object associated with the unit.</param>
    private Unit CreateUnitFromInput(RulesetType ruleset, Weapon weapon)
    {
        return new Unit
        {
            Id = 0,
            Ruleset = ruleset,
            Type = UnitInput.Type,
            Name = UnitInput.Name,
            MaxHealth = UnitInput.Health,
            CurrentHealth = UnitInput.Health,
            Weapon = weapon,
            Armor = UnitInput.Armor,
            AttackSkill = UnitInput.AttackSkill,
            DodgeChance = UnitInput.DodgeChance,
            BlockChance = UnitInput.BlockChance,

            AbilityIds = UnitInput.AbilityIds?
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries)
                .ToList() ?? []
        };
    }

    // ----------------------------------------------------------------------------------- UPDATE UNIT
    /// <summary>
    /// Updates an existing Unit based on the mouse selection.
    /// </summary>
    /// <param name="unit">The selected Unit to be edited.</param>
    /// <param name="weapon">The new Weapon assigned to the Unit (if applicable)</param>
    private void UpdateUnitFromInput(Unit unit, Weapon weapon)
    {
        unit.Name = UnitInput.Name;
        unit.Type = UnitInput.Type;
        unit.MaxHealth = UnitInput.Health;

        // Preserve current health, but never exceed the new maximum.
        unit.CurrentHealth = Math.Min(
            unit.CurrentHealth,
            UnitInput.Health);

        unit.Weapon = weapon;
        unit.Armor = UnitInput.Armor;
        unit.AttackSkill = UnitInput.AttackSkill;
        unit.DodgeChance = UnitInput.DodgeChance;
        unit.BlockChance = UnitInput.BlockChance;

        unit.AbilityIds = UnitInput.AbilityIds?
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries)
            .ToList() ?? [];
    }

    // ----------------------------------------------------------------------------------- ADD WEAPON
    /// <summary>
    /// Creates a new Weapon object from the provided Input.
    /// </summary>
    /// <param name="weaponType">The weaponType assigned to the weapon.</param>
    private Weapon CreateWeaponFromInput(RulesetType ruleset, WeaponType weaponType)
    {
        return new Weapon
        {
            Id = 0,
            Ruleset = ruleset,
            Name = WeaponInput.Name,
            Type = weaponType,
            Damage = WeaponInput.Damage,
            NumberOfAttacks = WeaponInput.NumberOfAttacks,
            AttackModifier = WeaponInput.AttackModifier,
            HitModifier = WeaponInput.HitModifier
        };
    }

    // ----------------------------------------------------------------------------------- UPDATE WEAPON
    /// <summary>
    /// Updates and existing Weapon based on the mouse selection.
    /// </summary>
    /// <param name="weapon">The selected Weapon to be edited.</param>
    /// <param name="weaponType">The new Weapon Type assigned to the Weapon (if applicable)</param>
    private void UpdateWeaponFromInput(Weapon weapon, WeaponType weaponType)
    {
        weapon.Name = WeaponInput.Name;
        weapon.Type = weaponType;
        weapon.Damage = WeaponInput.Damage;
        weapon.NumberOfAttacks = WeaponInput.NumberOfAttacks;
        weapon.AttackModifier = WeaponInput.AttackModifier;
        weapon.HitModifier = WeaponInput.HitModifier;
    }

    // ===========================================================================================================
    //                                                                                              UNIT BUTTONS
    // ===========================================================================================================
    
    // ----------------------------------------------------------------------------------- ADD UNIT
    /// <summary>
    /// Handles the POST request to add a new unit to the database.
    /// </summary>
    public async Task<IActionResult> OnPostAddUnitAsync()
    {
        var ruleset = GetRuleset();
        var weapon = await _dbContext.Weapons.FindAsync(UnitInput.WeaponId);
        
        if (!IsInputValid(nameof(UnitInput)) || weapon is null || ruleset is null || weapon.Ruleset != ruleset.Value)
        {
            if (weapon is null)
                ModelState.AddModelError("UnitInput.WeaponId", "Please select a valid weapon.");

            if (ruleset is null)
                ModelState.AddModelError(nameof(SelectedRuleset), "Please select a valid ruleset.");

            else if (weapon is not null && weapon.Ruleset != ruleset.Value)
                ModelState.AddModelError(nameof(UnitInput.WeaponId), "The chosen Weapon does not belong to the current Ruleset.");

            await LoadPageDataAsync();
            ShowUnitDialog = true;
            return Page();
        }

        var unit = CreateUnitFromInput(ruleset.Value, weapon);
        _dbContext.Units.Add(unit);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }

    // ----------------------------------------------------------------------------------- EDIT UNIT
    /// <summary>
    /// Handles the POST request to edit an existing unit.
    /// </summary>
    public async Task<IActionResult> OnPostEditUnitAsync()
    {
        if (!IsInputValid(nameof(UnitInput)) || SelectedUnitId <= 0)
        {
            if (SelectedUnitId <= 0)
                ModelState.AddModelError(string.Empty, "Select a unit to edit.");

            await LoadPageDataAsync();
            return Page();
        }

        var unit = await _dbContext.Units.FindAsync(SelectedUnitId);
        var weapon = await _dbContext.Weapons.FindAsync(UnitInput.WeaponId);

        if (unit is null || weapon is null)
        {
            if (unit is null)
                ModelState.AddModelError(string.Empty, "The selected unit could not be found.");
            if (weapon is null)
                ModelState.AddModelError("UnitInput.WeaponId", "Select an existing weapon.");

            await LoadPageDataAsync();
            return Page();
        }

        UpdateUnitFromInput(unit, weapon);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }

    // ----------------------------------------------------------------------------------- DELETE UNIT
    /// <summary>
    /// Handles the POST request to delete an existing unit from the database.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteUnitAsync()
    {
        if (SelectedUnitId <= 0)
            return BadRequest();

        var unit = await _dbContext.Units.FindAsync(SelectedUnitId);

        if (unit is null)
            return NotFound();

        _dbContext.Units.Remove(unit);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }

    // ===========================================================================================================
    //                                                                                            WEAPON BUTTONS
    // ===========================================================================================================

    // ----------------------------------------------------------------------------------- ADD WEAPON
    /// <summary>
    /// Handles the POST request to add a new weapon to the database.
    /// </summary>
    public async Task<IActionResult> OnPostAddWeaponAsync()
    {
        var ruleset = GetRuleset();
        var weaponType = GetWeaponType();

        if (!IsInputValid(nameof(WeaponInput)) || weaponType is null || ruleset is null)
        {
            if (weaponType is null)
                ModelState.AddModelError(nameof(SelectedWeapon), "Please select a valid weapon type.");

            if (ruleset is null)
                ModelState.AddModelError(nameof(SelectedRuleset), "Please select a valid Ruleset.");

            await LoadPageDataAsync();
            ShowWeaponDialog = true;
            return Page();
        }

        var weapon = CreateWeaponFromInput(ruleset.Value, weaponType.Value);

        _dbContext.Weapons.Add(weapon);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }

    // ----------------------------------------------------------------------------------- EDIT WEAPON
    /// <summary>
    /// Handles the POST request to edit an existing weapon in the database.
    /// </summary>
    public async Task<IActionResult> OnPostEditWeaponAsync()
    {
        var weaponType = GetWeaponType();

        if (!IsInputValid(nameof(WeaponInput)) || SelectedWeaponId <= 0 || weaponType is null)
        {
            if (SelectedWeaponId <= 0)
                ModelState.AddModelError(string.Empty, "Select a weapon to edit.");
            if (weaponType is null)
                ModelState.AddModelError("WeaponInput.Type", "Enter a valid weapon type.");

            await LoadPageDataAsync();
            return Page();
        }

        var weapon = await _dbContext.Weapons.FindAsync(SelectedWeaponId);

        if (weapon is null)
        {
            ModelState.AddModelError(string.Empty, "The selected weapon could not be found.");

            await LoadPageDataAsync();
            return Page();
        }

        UpdateWeaponFromInput(weapon, weaponType.Value);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }

    // ----------------------------------------------------------------------------------- DELETE WEAPON
    /// <summary>
    /// Handles the POST request to delete an existing weapon from the database.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteWeaponAsync()
    {
        if (SelectedWeaponId <= 0)
            return BadRequest();

        var weapon = await _dbContext.Weapons.FindAsync(SelectedWeaponId);

        if (weapon is null)
            return NotFound();

        _dbContext.Weapons.Remove(weapon);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage(new { SelectedRuleset });
    }
}