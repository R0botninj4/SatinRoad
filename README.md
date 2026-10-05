# SatinRoad

School project using .NET 10, Linq2db, SQLite, OpenAPI/Swagger,
Bun, React and TypeScript.

## Project structure

- `server/API`: Controllers and application configuration.
- `server/Service`: Business logic.
- `server/Infra`: Database access.
- `client`: React frontend and generated API client.

## Run with Docker

Requires Docker Desktop with Linux containers running.

From the repository root:

```powershell
docker compose up --build
```

- Frontend: http://localhost:3000
- Swagger: http://localhost:5188/swagger

Stop locally running frontend and backend servers first,
since they use the same ports.

Stop the containers with Ctrl+C. To remove the containers and
network, run:

```powershell
docker compose down
```

SQLite files are stored in the `sqlite-data` named volume,
mounted at `/app/data`. The volume survives container removal.
Using `docker compose down -v` also deletes the database volume.

After code changes, rebuild with `docker compose up --build`.

## Run without Docker

Requires .NET 10 SDK and Bun 1.3.14.

Start the backend from the repository root:

```powershell
dotnet run --project server/API
```

In another terminal:

```powershell
cd client
bun install
bun run dev
```

The backend must be running before starting the frontend.
The frontend dev command automatically generates the API client
from Swagger. Docker uses the generated client already in the repository.

## Backend accounts

All accounts have the same access; there are no roles in this implementation.
The existing category and item endpoints keep their current public access.

- `POST /api/auth/register`: send `username` and `password`; returns 201 with
  the user's ID and username, 400 for invalid input or 409 for a taken username.
- Usernames are 3–30 ASCII letters, numbers or underscores and are unique
  regardless of casing. Passwords are 12–128 characters and are stored using
  ASP.NET Core's PasswordHasher, never as plaintext or in API responses.
- `POST /api/auth/login`: send the same fields; returns the user's ID and
  username or 401 when the credentials are incorrect.

Validation used a separate temporary SQLite database: registration, duplicate
usernames, invalid input, successful login and incorrect passwords.
These scenarios are now covered by the automated backend test suite described below.

## Automated tests and quality assurance

We use **Test Last**: the application features were implemented first, then
automated tests were added against the existing requirements. For each feature
we identify successful behavior, rejection cases and important boundaries,
arrange the required data, perform the action, and assert both the response and
the resulting database state (Arrange–Act–Assert). Run the suite after changes
and before merging a pull request; add regression tests when fixing bugs.

The xUnit project `server/Service.Tests` exercises all five backend services
with Linq2db and a real, isolated temporary SQLite database per test. These are
service/database integration tests, rather than mocked unit tests. They use
the same table mappings and username uniqueness index as the application.
Database connections are disposed and temporary files deleted after each test;
the application database and Docker volume are never used.

Separate tests exercise the DTO validation annotations used by ASP.NET Core.
This distinction matters: services do not automatically execute those annotations.

Run from the repository root with the .NET 10 SDK (no running app or Docker needed):

```powershell
dotnet test SatinRoad.slnx --configuration Release
```

To also collect coverage:

```powershell
dotnet test SatinRoad.slnx --configuration Release --collect:"XPlat Code Coverage" --results-directory TestResults
```

The collector writes a `coverage.cobertura.xml` under `TestResults`.
Coverage helps identify untested behavior; 100% coverage is not the goal.
We prioritize logic that can affect stock, order totals, account access and data isolation.

| Area | Automated scenarios |
| --- | --- |
| Orders | Exact decimal totals, persisted order details, partial purchases, buying the last stock, insufficient stock, missing buyer/listing, sold-out purchases, buyer-specific history sorted newest first |
| Listings | Persisted owner/product/price/stock, trimmed description, missing user/product rejection, vendor display name and fallback |
| Accounts | Verifiable password hashing, case-insensitive duplicate rejection and login, wrong password, unknown user, user lookup, upgrading older password hashes |
| Categories and products | Trimmed category names, blank-name rejection, lookup, product ordering and empty catalog |
| Request validation | Nonpositive quantities/prices, missing listing/product IDs, minimum price, username and password length |

GitHub Actions (`.github/workflows/backend-tests.yml`) runs the Release test
suite on pull requests and pushes to `main`/`master`, and uploads coverage
results as a workflow artifact. The local suite is also run before submitting
the test changes for review.

Limitations: these tests do not exercise HTTP middleware, cookie authorization,
React/browser behavior, concurrent purchases or transaction rollback. Those
need additional API, browser or concurrency tests; passing this suite alone
does not demonstrate production readiness.

## Earlier setup validation

- Frontend and backend images build and start.
- React retrieves data from the backend through the generated client.
- Request failure and recovery were manually tested.
- A development-only SELECT 1 query verifies database connectivity,
  including inside Docker.
- A temporary file remained available after replacing a container,
  confirming that the named volume persists.

## Remaining work

Category administration, editing/restocking/removing listings, bonus features
and Lighthouse sustainability measurements still need work.

The current Docker setup is intended for running locally.
