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

## Current validation

- Frontend and backend images build and start.
- React retrieves data from the backend through the generated client.
- Request failure and recovery were manually tested.
- A development-only SELECT 1 query verifies database connectivity,
  including inside Docker.
- A temporary file remained available after replacing a container,
  confirming that the named volume persists.

## Remaining work

Product features, automated tests, testing methodology documentation
and Lighthouse sustainability measurements are not implemented yet.

The current Docker setup is intended for running locally.