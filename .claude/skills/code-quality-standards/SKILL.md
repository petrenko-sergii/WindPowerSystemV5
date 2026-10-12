---
name: code-quality-standards
description: Code quality checklist for WindPowerSystemV5 (ASP.NET Core 8 + Angular 17). Use when reviewing code, a diff or a pull request for correctness, maintainability, layering and test quality. Not for writing new features.
allowed-tools: Read, Grep, Glob, Bash(git diff *), Bash(git log *)
---

# Code Quality Standards

Review against this checklist. Report only concrete, verifiable problems, and label each finding **P0** (must fix: bug, data loss, broken contract), **P1** (should fix: maintainability, missing tests) or **P2** (nice to have).

## Backend (ASP.NET Core 8)

1. **Layering**: Controllers -> Services (`Services/Interfaces`) -> Repositories (SQL only) -> DbContext. Controllers must not use `DbContext` or repositories directly.
2. **DI**: every new service/repository is registered in `Config/ServiceModule.cs`.
3. **DTOs and mapping**: entities are never returned from controllers; use `Data/DTOs` with an AutoMapper profile in `Mappings/`. Request shapes live in `ViewModels/`.
4. **Errors**: throw `NotFoundException` / `BadRequestException` (`Utils/Exceptions`) instead of returning ad-hoc status codes or swallowing exceptions. No empty `catch` blocks.
5. **Async**: `async`/`await` end to end, no `.Result` / `.Wait()`, `CancellationToken` passed through for I/O calls, `Async` suffix on async methods.
6. **EF Core**: no queries inside loops (N+1), use `AsNoTracking()` for read-only queries, project to DTOs where possible, schema changes come with a migration in `Data/Migrations`.
7. **Naming and size**: clear names, small methods, no dead code or commented-out code, no magic strings/numbers (use constants or options classes).
8. **Logging**: use `ILogger<T>` with message templates (`"City {CityId} not found"`), never string interpolation, and never log secrets or personal data.
9. **Nullability**: nullable reference types respected; no `!` suppressions without a reason.

## Frontend (Angular 17)

1. HTTP calls go through services extending `BaseService<T>`; components do not call `HttpClient` directly.
2. Forms extend `base-form.component.ts`; validation lives in the form, not scattered in templates.
3. Every RxJS subscription is cleaned up (`async` pipe, `takeUntil`, `takeUntilDestroyed`); no nested subscribes.
4. No `any` without a comment explaining why; models/interfaces are typed.
5. Routes that need auth are protected by `auth.guard.ts`.
6. Templates: no heavy logic or function calls in bindings; `trackBy` on `*ngFor` lists.

## Tests

1. Every test in `WindPowerSystemV5.Server.Tests` follows Arrange-Act-Assert with `// Arrange`, `// Act`, `// Assert` comments (`// Act & Assert` for a single expression, see `.claude/rules/tests.md`).
2. Test names follow `Method_Scenario_ExpectedResult`.
3. New logic has tests for the happy path, the not-found/validation path and at least one edge case. Repositories are mocked with NSubstitute; EF Core InMemory is used for data access.
4. No tests that depend on real network, real databases or the current time.

## Housekeeping

1. User-visible changes are recorded in the project's `CHANGELOG.md`.
2. Commit messages follow `.claude/rules/commit-messages.md` (`type: Subject`, no scope).

## Output format

```
[P1] WindPowerSystemV5.Server/Services/CityService.cs:42
Problem: <what is wrong and why it matters>
Fix: <concrete suggestion, with a short code snippet if useful>
```

End with a one-line verdict: **Approve**, **Approve with comments** or **Request changes**.

## Example

Bad:
```csharp
var city = _context.Cities.Where(c => c.Id == id).ToList()[0];   // throws on empty, loads all matches
```
Good:
```csharp
var city = await _repository.GetByIdAsync(id, cancellationToken)
    ?? throw new NotFoundException($"City {id} not found");
```
