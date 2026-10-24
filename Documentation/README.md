# Setup and Tests

## Requirements

- .NET 10 SDK
- Git to clone the repository

VS Code is optional. Any editor that supports the project can be used.

## Run AUBE

From the repository root, run:

```sh
dotnet build AUBE.csproj
dotnet run --project AUBE.csproj
```

The first launch sets up the SQLite database and, in Development mode, adds two sample units and their training weapon.

On later launches, AUBE keeps existing data and checks for database updates. Sample data is only added in Development mode when the Units table is empty.

Production mode does not automatically add sample data.

## Run Tests

Run the automated tests with:

```sh
dotnet test Tests/AUBE.Tests.csproj
```

The tests check battle simulation behavior and database initialization. They use test data rather than modifying the application's normal database.

## How To Use

### Index (Home Page)

The home page consists of the chosen Scenario (There is only one available at this time) and drop downs for both the attacker and defender.

Below that is where you can set the number of iterations you want to test.

After you have chosen an Attacker, Defender, and Iterations, hit Run Simulation to generate the combat report.
The Report displays basic info, but can be expanded to see additional details by clicking on in in the Report list.

The Report list can be filtered by a Units name to retrieve just reports that they were a part of.

### Database

The Database page provides a simple way for you to preform CRUD operations with Units and Weapons within the Database.

First, ensure you select the Ruleset from the dropdown, as the tables use that to give you specific instructions when creating new objects.

Second, create a new Weapon, if it doesnt currently exist in the database. Click Add to create the Weapon and store it in the database.

After your chosen weapon is created, you can create your Unit. When creating the Unit, the Weapon dropdown will allow you to select the Weapon you created earlier.

At any time, you may click on a Unit or Weapon youd like to modify/delete and click the associated button.