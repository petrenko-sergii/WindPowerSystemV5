# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Learning project (follows De Sanctis, *ASP.NET Core 8 and Angular*): ASP.NET Core 8 backend + Angular 17 SPA, using several databases at once. Solution: `WindPowerSystemV5.sln`.

- `WindPowerSystemV5.Server` – ASP.NET Core 8 API (also hosts the SPA via `Microsoft.AspNetCore.SpaProxy`)
- `WindPowerSystemV5.Server.Tests` – xUnit + NSubstitute, EF Core InMemory
- `windpowersystemv5.client` – Angular 17 (NgModule-based, Angular Material, Karma/Jasmine)
- `terraform/` – Azure infra; `docs/WPS.postman_collection.json` – API collection

## Commands

Backend (from repo root):
- Run: `dotnet run --project WindPowerSystemV5.Server` (launches the Angular dev server via SpaProxy → `npm start`, https://localhost:4200)
- Test all: `dotnet test`
- Single test: `dotnet test --filter "FullyQualifiedName~CitiesControllerTests.GetCity_CityExistsInDatabase_ReturnsCity"`
- EF migrations: `dotnet ef migrations add <Name> --project WindPowerSystemV5.Server -o Data/Migrations`, then `dotnet ef database update --project WindPowerSystemV5.Server`

Frontend (from `windpowersystemv5.client`):
- `npm start` (runs `aspnetcore-https.js` first to create dev certs, then `ng serve --ssl`), `npm run build`, `npm test`
- Single spec: `npx ng test --include='**/app.component.spec.ts'`

## Architecture

**Polyglot persistence** – each store has its own access path:
- **SQL Server** (EF Core, `ApplicationDbContext`, also ASP.NET Identity with `ApplicationUser`): cities, countries, turbines, turbine types. Accessed via `Data/Repositories` → `Services`. Serilog also logs to a `LogEvents` table here.
- **Azure Cosmos DB** (`Services/CosmosDbContext`, models in `Data/CosmosDbModels`): maintenance records, turbine config snapshots.
- **MongoDB** (`Data/MongoDbModels`, settings `Config/NewsDbSettings`, config section `MongoDB`): news and comments. Seed via `Data/SeedScripts/SeedMongoDB.bat` (needs `mongosh` on PATH).
- **Azure Blob Storage** (`Services/BlobStorageService`, options section `AzureBlobStorage`): turbine type info files.

**Backend layering**: Controllers → Services (`Services/Interfaces`) → Repositories (SQL only) → DbContext. Entities (`Data/Models`) are mapped to DTOs (`Data/DTOs`) via AutoMapper profiles in `Mappings/` (registered in `AutomapperConfig`); request shapes live in `ViewModels/`. DI registration is in `Config/ServiceModule.cs` (`RegisterRepositories`/`RegisterServices`) — add new services/repos there. Controllers/services throw `NotFoundException`/`BadRequestException` (`Utils/Exceptions`), which a global exception handler in `Program.cs` turns into 404/400 JSON `{ error }`.

**API surface**: REST controllers under `api/...`, plus HotChocolate GraphQL at `/api/graphql` (`Data/GraphQL`, turbines), SignalR hub `/api/health-hub` with health checks at `/api/health`, and `HEAD /api/heartbeat` (used by the frontend to detect backend availability). Auth is JWT bearer (`JwtHandler`, `AccountController`); GraphQL has authorization enabled.

**Configuration**: `appsettings.json` contains only `override-this` placeholders for secrets (SQL connection, Cosmos, Mongo, JWT, Blob). Supply real values via user-secrets / environment variables, not in committed files. `AllowedCORS` feeds the `AngularPolicy` CORS policy.

**Frontend**: feature folders under `src/app` (turbines, turbine-types, cities, countries, news, auth, health-check…), wired in `app-routing.module.ts` / `app.module.ts`. Services extend `BaseService<T>` (`base.service.ts`), which prefixes URLs with `environment.baseUrl`; forms extend `base-form.component.ts`. Auth is handled by `auth/auth.interceptor.ts` (attaches JWT), `auth.guard.ts`, `auth.service.ts`. Both REST and GraphQL are used for client↔server communication.

## Conventions

- Each project keeps a `CHANGELOG.md` (`WindPowerSystemV5.Server/`, `windpowersystemv5.client/`).
- Tests use InMemory EF + NSubstitute substitutes for repositories (see `Server.Tests/CitiesControllerTests.cs`).
