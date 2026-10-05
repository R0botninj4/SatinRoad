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
Registration input validation is covered by the unit tests described below.

## Unit tests

We use **Test Last**: write the feature first, then write tests for its logic.
The tests use xUnit with `[Fact]`, `[Theory]` and Arrange–Act–Assert, following
the same style as our earlier SuperChocolateMilk tests.

Run the tests from the repository root:

```powershell
dotnet test SatinRoad.slnx
```

- `OrderHelpersTests`: total price, decimal prices, remaining stock,
  buying the last item and rejecting purchases with insufficient stock.
- `RequestValidationTests`: quantities, listing IDs, product IDs, prices
  and username/password lengths.

The price and stock calculations are extracted into `OrderHelpers` and used
by `OrderService`. Tests call these methods directly, without a database or mocks.
We test normal cases, invalid input and boundaries rather than aiming for 100% coverage.
After changing a feature, update its tests and run them before merging.
GitHub Actions also runs them on pull requests and pushes to main/master.

These are focused unit tests; database, login flows and browser behavior are
not covered by this suite.

## Earlier setup validation

- Frontend and backend images build and start.
- React retrieves data from the backend through the generated client.
- Request failure and recovery were manually tested.
- A development-only SELECT 1 query verifies database connectivity,
  including inside Docker.
- A temporary file remained available after replacing a container,
  confirming that the named volume persists.

## Remaining work

Category administration, editing/restocking listings, bonus features
and Lighthouse sustainability measurements still need work.

The current Docker setup is intended for running locally.
