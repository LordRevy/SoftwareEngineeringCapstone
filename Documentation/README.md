
# AUBE: Automated Unit Balance Engine

## Project Overview

Users can manage units and weapons, run D6 melee matchups for up to 10,000 iterations, and review or search recent simulation reports. Repeated simulations help game designers and developers compare unit and weapon balance without manually resolving every matchup.

## Project Links and Documentation

- **GitLab repository:** [AUBE](https://gitlab.com/wgu-gitlab-environment/student-repos/tboyc27/d424-software-engineering-capstone/-/tree/working)
- **Hosted application:** Add the deployment URL if the app is hosted.
- **ERD:** [View the database diagram](Screenshots/AUBE_ERD.png)
- **UML class diagram:** [View the UML diagram](Screenshots/UML.png)
- **Test results screenshot:** [View the Tests](Screenshots/SuccessfulTests_Full.png)

## Setup and Installation

### Requirements
- .NET 10 SDK
- Git

VS Code or another compatible editor is optional.

### Run AUBE

From the repository root, run:

```sh
dotnet build AUBE.csproj
dotnet run --project AUBE.csproj
```

Open the URL printed by the application. With the included Development launch profile, the app uses `https://localhost:7068` or `http://localhost:5044`. On startup, AUBE applies database migrations. Development mode adds two sample units and a weapon when the Units table is empty; other environments do not seed sample data.

## User Guide

### Sign In

Register an account or sign in using the links in the navigation. Sign-in is required to access the Database page.

### Home Page and Simulations
Select two different D6 units, choose 1 to 10,000 iterations, and run the simulation. The report summarizes wins, win rates, damage, draws, and rounds. Recent reports are listed below; search by unit name and select a report to view its details.

### Database Management
Choose a ruleset, then add weapons and units. A unit must reference a weapon in the same ruleset. Select an existing row to edit or delete it. The page is available to signed-in users.

## Testing

See the `TestPlan.md` for a detailed breakdown of the tests preformed.

## Developer's Guide

### Project Structure
- `Pages/`: Razor Pages for the simulation and database interface.
- `Objects/`: Domain models, enums, and validated form input models.
- `Mechanics/`: Battle simulation and ruleset logic.
- `Data/` and `Migrations/`: Entity Framework Core context, initialization, and schema migrations.
- `Tests/`: Automated xUnit tests.
- `wwwroot/`: CSS, JavaScript, and front-end libraries.

### Architecture and Design
The app uses ASP.NET Core Razor Pages, Entity Framework Core with SQLite, and ASP.NET Core Identity. `BattleSimulator` runs battles through the `IRuleset` interface; `D6Ruleset` currently provides the combat rules. This interface allows combat behavior to be supplied by other rulesets. Page models handle requests and use the database context for data access.

### Database Design
See the [ERD](Screenshots/AUBE_ERD.png). Units reference a weapon. Simulation reports store matchup names and unit IDs with aggregate results; the report IDs are stored as values rather than foreign-key relationships. This was done intentionally so records arent modified if Units are edited or deleted.

### Extending AUBE
To change the database schema, update the relevant entity or context, add an EF Core migration, and verify the app starts and the tests pass.

To add a ruleset, implement a new Ruleset in /Mechanics and ensure it inherits the interface `IRuleset`, add a value to `RulesetType` in `Objects/Enums.cs`. An enum value alone only makes the ruleset available for selection; it does not enable it in simulations. Add tests for the new rules.

Features such as abilities or environmental modifiers may require changes to the domain models, input validation, user interface, database, and simulation logic. The existing ruleset interface provides an extension point, but the work depends on the feature.