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
    //                                                                                          START SIMULATION
    // ===========================================================================================================

    /// <summary>
    /// Validates and preforms checks on selected Units before beginning simulation.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        await LoadUnitsAsync();

        if (FirstUnitId <= 0 || SecondUnitId <= 0 || FirstUnitId == SecondUnitId)
            ModelState.AddModelError(string.Empty, "Select two different units to run a simulation.");

        if (!ModelState.IsValid)
            return Page();

        var (first, second) = await GetSelectedUnitsAsync();

        if (first is null || second is null || first.Ruleset != RulesetType.D6 || second.Ruleset != RulesetType.D6)
        {
            ModelState.AddModelError(string.Empty, "Both selected units must exist and use the D6 ruleset.");
            return Page();
        }

        await RunAndSaveSimulationAsync(first, second);
        await LoadRecentReportsAsync();

        return Page();
    }

    // ===========================================================================================================
    //                                                                                            HELPER METHODS
    // ===========================================================================================================

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

    // ----------------------------------------------------------------------------------- LOAD REPORTS
    /// <summary>
    /// Gets the first 25 reports in the database.
    /// </summary>
    private async Task LoadRecentReportsAsync()
    {
        RecentReports = await _dbContext.Reports
            .AsNoTracking()
            .OrderByDescending(report => report.CreatedAt)
            .Take(25)
            .ToListAsync();
    }

    // ----------------------------------------------------------------------------------- GET SELECTED UNITS
    /// <summary>
    /// Gets the Selected Units from the Database and returns them.
    /// </summary>
    private async Task<(Unit? First, Unit? Second)> GetSelectedUnitsAsync()
    {
        var selectedIds = new[] { FirstUnitId, SecondUnitId };

        var selectedUnits = await _dbContext.Units
            .AsNoTracking()
            .Include(unit => unit.Weapon)
            .Where(unit => selectedIds.Contains(unit.Id))
            .ToListAsync();

        var first = selectedUnits.SingleOrDefault(unit => unit.Id == FirstUnitId);
        var second = selectedUnits.SingleOrDefault(unit => unit.Id == SecondUnitId);

        return (first, second);
    }   

    // -------------------------------------------------------------------------------- CREATE AND SAVE REPORT
    /// <summary>
    /// Gets the report data, and saves it as a report in the database.
    /// </summary>
    private async Task RunAndSaveSimulationAsync(Unit first, Unit second)
    {
        Report = _simulator.RunSimulation(first, second, Iterations);

        _dbContext.Reports.Add(Report);
        await _dbContext.SaveChangesAsync();
    }

    // ----------------------------------------------------------------------------------- GET REPORT DETAILS
    /// <summary>
    /// Takes a report ID and gets the full list of details.
    /// </summary>
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