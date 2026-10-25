# Testing

## Unit Tests
Test Scripts are in the `Tests` folder and use xUnit. They verify simulation results and iteration validation, D6 dodge/block behavior, database initialization and seeding, and authorization on the Database page. Additional Tests will be added as the project scales.

Simulation tests use test objects and a deterministic ruleset where needed. Database initialization tests use a temporary in-memory SQLite database, so the application's normal database is not modified.

## Testing Results
Latest run: 7 tests passed and 0 failed with `dotnet test Tests/AUBE.Tests.csproj`. No code changes were needed in response to this run. [TestResults] (Screenshots/TestResults.png)
 test plan for a unit test, including screenshots

## Changes Made


## Running Tests
Basic Test
```sh
dotnet test Tests/AUBE.Tests.csproj
```

Detailed Test
```
dotnet test Tests/AUBE.Tests.csproj --logger "console;verbosity=detailed"
```