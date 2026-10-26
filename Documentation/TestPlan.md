# Testing

## Unit Tests
Test Scripts are in the `Tests` folder and use xUnit. They verify simulation results and iteration validation, D6 dodge/block behavior, database initialization and seeding, and authorization on the Database page. Additional Tests will be added as the project scales.

Simulation tests use test objects and a deterministic ruleset where needed. Database initialization tests use a temporary in-memory SQLite database, so the application's normal database is not modified.

## Testing Results
Latest run: 7 tests passed and 0 failed with `dotnet test Tests/AUBE.Tests.csproj`. No code changes were needed in response to this run. [Test Results](Screenshots/SuccessfulTests_Full.png)

## Changes Made
During the `RecordsWinsAndDoesNotChangeSavedUnits` Test, an error was identified where some units would win every encounter. Further investigation revealed that a value of 0 for DodgeSkill and/or BlockSkill resulted in a automatic pass for every check. This error would certainly make the software unreliable and was fixed immediately. For good measure, `D6Ruleset_ZeroDodgeAndBlockChanceDoNotAutomaticallySucceed` was created to monitor and validate the issue if future changes ever trigger this again.

## Running Tests
Basic Test
```sh
dotnet test Tests/AUBE.Tests.csproj
```

## Detailed Test
```sh
dotnet test Tests/AUBE.Tests.csproj --logger "console;verbosity=detailed"
```