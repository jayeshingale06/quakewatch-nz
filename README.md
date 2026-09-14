# QuakeWatch NZ

A web application that shows recent New Zealand earthquakes, using live public data from [GeoNet](https://www.geonet.org.nz/).

This is a rebuild of an earthquake data explorer I originally wrote in R Shiny during my Master of Computer Science at Victoria University of Wellington. The original read a static USGS dataset of roughly 56,000 events. This version is a React and TypeScript front end on a C# REST API, backed by a database, containerised, and deployed to Azure through a CI/CD pipeline.

**Live application:** https://jayeshingale06.github.io/quakewatch-nz/
**API:** https://quakewatch-nz.onrender.com/api/quakes

> Note on first load: the API runs on a free tier that sleeps after fifteen
> minutes without traffic. The first request after a quiet period can take up
> to a minute while the container wakes and refreshes its cache from GeoNet.
> Loads after that are fast.

---

## What it does

- Fetches earthquakes from the GeoNet public API
- Stores them in a database and serves from that copy, refreshing only when the data is more than ten minutes old
- Classifies each earthquake into a shaking severity band derived from its Modified Mercalli Intensity
- Shows the list with colour coded severity, summary figures, and filtering by band
- Handles loading, empty and error states explicitly

---

## Architecture

```
Browser
   |
   |  HTTPS
   v
React + TypeScript  (nginx container)
   |
   |  REST / JSON
   v
ASP.NET Core API    (container)
   |                     |
   |  Entity Framework   |  HttpClient
   v                     v
Database            api.geonet.org.nz
(PostgreSQL or SQLite)
```

The API is not a pass through. It is the only thing that talks to GeoNet, which means the front end is insulated from GeoNet's data format, GeoNet is not called once per visitor, and the severity rules live in exactly one place.

---

## Technology

| Layer | Technology |
| --- | --- |
| Front end | React 19, TypeScript, Vite |
| Back end | C#, ASP.NET Core 10, Minimal APIs |
| Data access | Entity Framework Core |
| Database | PostgreSQL in Docker, SQLite for local development |
| Tests | xUnit, Vitest, React Testing Library |
| Containers | Docker, multi stage builds, nginx |
| Hosting | Azure Container Apps |
| CI/CD | GitHub Actions, GitHub Container Registry |

---

## Design decisions

These are the choices I would want to talk through in a code review.

**The API translates GeoNet's shape into its own.** GeoNet returns GeoJSON, where each earthquake is a Feature wrapped around a geometry and a properties object, and magnitudes arrive with fifteen decimal places. `GeoNetService` converts that into the application's own `Quake` record at the boundary, rounding magnitude and depth and filling in defaults for missing fields. If GeoNet changes their format, one method needs updating rather than every file that touches earthquake data.

**Severity is calculated, never stored.** `Quake.Severity` is a computed property derived from the MMI value using a switch expression with a catch all arm, so every possible MMI lands in exactly one band and no value can fall through unclassified. Storing severity separately would allow it to disagree with the MMI it came from.

**Severity is calculated on the server, not the client.** An earlier version of the front end had its own copy of the band rules in JavaScript. That is two sources of truth for one business rule. The API now sends `severity` with every earthquake and the front end simply displays it.

**The database is a cache, so the deployed version uses ephemeral storage.** Every row can be rebuilt from GeoNet at any time, and each row records when it was fetched. That makes a managed database server unnecessary for the hosted version: losing the data on restart costs nothing but one refetch. The connection string is read from configuration, so the same code runs against PostgreSQL in Docker and SQLite in a container, with no code change.

**Writes are an upsert, not an insert.** GeoNet revises earthquakes as human analysts review them, so the same `publicID` can arrive with a changed magnitude, depth or quality. Rows are matched on GeoNet's own identifier, which is used as the primary key, so duplicates are impossible at the database level.

**The React data fetching lives in a custom hook.** `useQuakes` owns the loading flag, the error message, the cleanup and the retry. Components receive data and render it. This is what makes the hook testable without a browser and the components testable without a network.

**Race conditions are handled explicitly.** The fetch effect sets a flag in its cleanup function, so a slow response that arrives after a newer one has already rendered is discarded rather than overwriting fresher data.

**CORS lists specific origins.** `AllowAnyOrigin` would have been shorter. The allowed origins are read from configuration so they can differ between local development, Docker and Azure without a code change.

**Containers run as a non root user and the images are multi stage.** The .NET SDK and Node are present only in the build stage. The images that ship contain the runtime and the built output, nothing else.

---

## Running it locally

### With Docker (everything at once)

Requires Docker Desktop.

```bash
docker compose up --build
```

- Application: http://localhost:8080
- API: http://localhost:5055/api/quakes

This starts PostgreSQL, the API and the web application together.

### Without Docker

Requires the .NET 10 SDK and Node.js 24 or later.

Terminal one, the API:

```bash
cd backend/QuakeWatch.Api
dotnet watch
```

Terminal two, the front end:

```bash
cd frontend-react
npm install
npm run dev
```

- Application: http://localhost:5173
- API: http://localhost:5055/api/quakes

With no connection string configured, the API uses a local SQLite file, so no database server is needed.

---

## Running the tests

Back end:

```bash
dotnet test backend/QuakeWatch.Api.Tests/QuakeWatch.Api.Tests.csproj
```

Front end:

```bash
cd frontend-react
npm test
npm run typecheck
```

The back end tests cover the severity bands at every boundary value, and the GeoNet translation, using a stubbed `HttpMessageHandler` so no network call is made. The front end tests cover the time formatter, the `QuakeCard` component, and the `useQuakes` hook with a mocked API module.

---

## Project structure

```
.
|-- backend/
|   |-- QuakeWatch.Api/           ASP.NET Core API
|   |   |-- Models/               Quake, and GeoNet's own shapes
|   |   |-- Data/                 EF Core entity and DbContext
|   |   |-- Services/             GeoNetService, QuakeStore
|   |   `-- Dockerfile
|   `-- QuakeWatch.Api.Tests/     xUnit tests
|-- frontend-react/
|   |-- src/
|   |   |-- api/                  the only file that calls the API
|   |   |-- hooks/                useQuakes
|   |   |-- components/           presentation only
|   |   |-- utils/                pure functions
|   |   `-- types.ts              shared TypeScript types
|   |-- nginx.conf
|   `-- Dockerfile
|-- frontend/                     the original vanilla HTML/CSS/JS version
|-- deploy/                       Azure setup and teardown scripts
|-- .github/workflows/            CI and deployment pipelines
`-- docker-compose.yml
```

`frontend/` is the plain HTML, CSS and JavaScript version this project started as. It is kept because it is useful to compare against the React version.

---

## Deployment

Every push to `main` runs `.github/workflows/ci.yml`, which builds the API, runs both test suites, type checks the front end and builds both Docker images on a clean machine.

Deployment is then handled by the hosting platforms, each watching the repository:

- The API is deployed from `backend/QuakeWatch.Api/Dockerfile` to a managed container platform, described declaratively in `render.yaml`. It reads the `PORT` environment variable, so the same image runs unchanged on any host that sets it.
- The front end is built with `npm run build` and served as static files from a CDN, with `VITE_API_BASE` supplied at build time because Vite resolves environment variables when it compiles.
- No managed database is provisioned. With no connection string the API falls back to SQLite inside the container, which is acceptable because the data is a rebuildable cache of GeoNet.

An Azure Container Apps path is also included and ready to use: `deploy/azure-setup.sh` creates the resources, `azure-status.sh` lists them, `azure-teardown.sh` removes them, and `.github/workflows/deploy.yml` builds images, publishes them to GitHub Container Registry tagged by commit SHA, and deploys that exact SHA rather than a moving `latest` tag. That workflow is currently manual-only; uncommenting its push trigger switches it on.

---

## Known limitations

- The API response is trusted rather than validated at runtime. TypeScript checks the code, not the data arriving over the network. A schema validator such as Zod would close that gap.
- The database schema is created with `EnsureCreated` rather than EF Core migrations, which is adequate here because the data is a rebuildable cache, but would not be acceptable for data that matters.
- There are no end to end tests. The components, hook, and API logic are tested in isolation, but nothing exercises the full path from browser to GeoNet.
- The GeoNet request fetches up to the API's maximum result count. Paging is not implemented.

---

## Data source

Earthquake data is provided by [GeoNet](https://www.geonet.org.nz/), a collaboration between Earth Sciences New Zealand, the Earthquake Commission and Land Information New Zealand. GeoNet data is made freely available.
