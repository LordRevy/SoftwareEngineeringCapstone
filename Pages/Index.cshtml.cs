using AUBE.Data;
using AUBE.Mechanics;
using AUBE.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AUBE.Pages;

public class IndexModel : PageModel
{
    // ===========================================================================================================
    //                                                                                  CONSTRUCTOR & PROPERTIES
    // ===========================================================================================================
    private readonly AubeDbContext _dbContext;
    private readonly BattleSimulator _simulator;

    public IndexModel(AubeDbContext dbContext, BattleSimulator simulator)
    {
        _dbContext = dbContext;
        _simulator = simulator;
    }

    public List<Unit> Units { get; private set; } = [];
    public Report? Report { get; private set; }
    public List<Report> RecentReports { get; private set; } = [];

    [BindProperty]
    [Range(1, 10000, ErrorMessage = "Choose between 1 and 10,000 iterations.")]
    public int Iterations { get; set; } = 1000;

    [BindProperty]
    public int FirstUnitId { get; set; }

    [BindProperty]
    public int SecondUnitId { get; set; }

    public async Task OnGetAsync()
    {
        await LoadUnitsAsync();
        await LoadRecentReportsAsync();
    }

    // ===========================================================================================================
    //                                                                                            HELPER METHODS
    // ===========================================================================================================

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadUnitsAsync();

        if (FirstUnitId <= 0 || SecondUnitId <= 0 || FirstUnitId == SecondUnitId)
            ModelState.AddModelError(string.Empty, "Select two different units to run a simulation.");

        if (!ModelState.IsValid)
            return Page();

        var selectedIds = new[] { FirstUnitId, SecondUnitId };
        var selectedUnits = await _dbContext.Units
            .AsNoTracking()
            .Include(unit => unit.Weapon)
            .Where(unit => selectedIds.Contains(unit.Id))
            .ToListAsync();

        var first = selectedUnits.SingleOrDefault(unit => unit.Id == FirstUnitId);
        var second = selectedUnits.SingleOrDefault(unit => unit.Id == SecondUnitId);

        if (first is null || second is null || first.Ruleset != RulesetType.D6 || second.Ruleset != RulesetType.D6)
        {
            ModelState.AddModelError(string.Empty, "Both selected units must exist and use the D6 ruleset.");
            return Page();
        }

        Report = _simulator.RunSimulation(first, second, Iterations);
        _dbContext.Reports.Add(Report);
        await _dbContext.SaveChangesAsync();
        await LoadRecentReportsAsync();
        return Page();
    }

    // ----------------------------------------------------------------------------------- LOAD UNITS
    /// <summary>
    /// Takes the selected Units and adds them to a list.
    /// </summary>
    private async Task LoadUnitsAsync()
    {
        Units = await _dbContext.Units
            .AsNoTracking()
            .Include(unit => unit.Weapon)
            .Where(unit => unit.Ruleset == RulesetType.D6)
            .OrderBy(unit => unit.Name)
            .ToListAsync();
    }

    private async Task LoadRecentReportsAsync()
    {
        RecentReports = await _dbContext.Reports
            .AsNoTracking()
            .OrderByDescending(report => report.CreatedAt)
            .Take(25)
            .ToListAsync();
    }

    public async Task<PartialViewResult> OnGetReportDetailsAsync(int id)
    {
        var report = await _dbContext.Reports
            .AsNoTracking()
            .FirstOrDefaultAsync(report => report.Id == id);

        if (report is null)
            return Partial("_ReportNotFound");

        return Partial("_ReportDetails", report);
    }
}