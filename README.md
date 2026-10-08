# SatinRoad

School marketplace project built with .NET 10, Linq2DB, SQLite, Bun, React and TypeScript.

## Where the code lives

- `server/API`: HTTP controllers and application setup.
- `server/Service`: marketplace rules and business logic.
- `server/Infra`: database models and access.
- `server/Service.Tests`: xUnit tests.
- `client/src/pages`: React pages. `client/src/api` is generated from Swagger; edit the server DTOs instead of editing generated API types by hand.

The normal request path is **React → API controller → service → database**. Keep rules in `Service`, not in a controller or a React component. The tests use xUnit and Arrange–Act–Assert, as in the class exercises.

## Marketplace rules

The adjustable values are together in [`server/Service/MarketplaceRules.cs`](server/Service/MarketplaceRules.cs):

| Value | Current setting | Used by |
| --- | --- | --- |
| `FbiChancePercent` | `1` (1% per successful purchase) | `RandomFbiCheck` in [`IFbiCheck.cs`](server/Service/IFbiCheck.cs) |
| `FeaturedVendorSalesThreshold` | `100` (featured after **more than** 100 orders, starting with order 101) | `ListingService.GetAll` |
| `LoyaltyDiscountInterval` | `11` (every 11th order with the same seller) | `OrderHelpers` |
| `LoyaltyDiscountMultiplier` | `0.80m` (20% off that order) | `OrderHelpers` |

For example, set `FbiChancePercent` to `10` to test a 10% chance, or `FeaturedVendorSalesThreshold` to `10` to test the featured marker after 11 sales. Put them back to `1` and `100` before merging. After changing server code, rebuild/restart locally or run `fly deploy --ha=false -a satinroad` to update Fly. A source-code change alone does not change a running container or Fly Machine.

An FBI purchase still completes. It permanently flags the seller, removes all their listings, and prevents new listings. The seller can still sign in and buy; their profile shows a disconnect notice.

## Accounts

All accounts have the same access; there are no roles. `POST /api/auth/register` creates an account and `POST /api/auth/login` starts a cookie session. Usernames are 3–30 ASCII letters, numbers or underscores, unique regardless of case. Passwords are 12–128 characters and stored as hashes. `GET /api/auth/me` restores the current user after a page reload, including the seller's shutdown status.

## Run locally with Docker

Requires Docker Desktop with Linux containers. From the repository root:

```powershell
docker compose up --build -d
```

Open <http://localhost:3000>; Swagger is at <http://localhost:5188/swagger>. Stop the containers with:

```powershell
docker compose stop
```

Start the same containers later with:

```powershell
docker compose start
```

After code changes, use `docker compose up --build -d` again. To remove the containers and network, use `docker compose down`. The `sqlite-data` volume holds the SQLite database and survives these commands. **`docker compose down -v` deletes that volume and its data.**

## Run locally without Docker

Requires the .NET 10 SDK and Bun. Stop Docker containers first if they are using ports 3000 or 5188. In one terminal, from the repository root:

```powershell
dotnet run --project server/API/API.csproj
```

In a second terminal:

```powershell
cd client
bun install
bun run dev
```

Start the API first: `bun run dev` generates `client/src/api/Api.ts` from its Swagger document. Open <http://localhost:3000>. Stop each process with **Ctrl+C** in its terminal; run the same commands to start them again. Without Docker, the SQLite file is `satinroad.db` in the API process's working directory (the default connection string is in [`server/API/appsettings.json`](server/API/appsettings.json)).

## Run on Fly.io

The app name in [`fly.toml`](fly.toml) is `satinroad`, so the URL is <https://satinroad.fly.dev> while its Machine is running. Fly uses [`Dockerfile.fly`](Dockerfile.fly) to build the frontend and API together. Its SQLite file is on the `satinroad_data` volume at `/app/data`; this app should run **one Machine** because separate Fly volumes do not automatically share SQLite data.

To deploy new code from the repository root:

```powershell
fly deploy --ha=false -a satinroad
```

To start or stop the existing deployment without deleting its app or database, find its current Machine ID (it can change after a deploy):

```powershell
fly machine list -a satinroad
$machineId = "PASTE_ID_FROM_LIST"
```

Stop it:

```powershell
fly machine stop $machineId -a satinroad
fly machine list -a satinroad
```

Start it again:

```powershell
fly machine start $machineId -a satinroad
fly machine list -a satinroad
```

Stop **every** Machine shown in the list if there is more than one. The current `fly.toml` has `auto_stop_machines = "off"` and `auto_start_machines = false`: Fly will leave a started Machine running until you stop it, and a web request will not restart a stopped Machine. Stopping preserves the attached volume. A stopped Machine can return a 503 page until you start it again; allow a short moment after starting. The volume may still incur charges while the Machine is stopped; check [Fly's current pricing](https://fly.io/docs/about/pricing/).

## Verify changes

From the repository root:

```powershell
dotnet test SatinRoad.slnx
cd client
bunx tsc --noEmit
bun run build
```

The service tests cover pricing boundaries and FBI purchase effects using a temporary SQLite database. Run them after changing marketplace rules. GitHub Actions also runs tests for pull requests and pushes to `main`.
