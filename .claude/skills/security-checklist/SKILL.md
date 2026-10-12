---
name: security-checklist
description: Security review checklist for WindPowerSystemV5 based on the OWASP Top 10 and ASP.NET Core guidance. Use for security audits, or when reviewing code that touches authentication, authorization, user input, configuration, secrets, databases or external services.
allowed-tools: Read, Grep, Glob, Bash(git diff *), Bash(git log *)
---

# Security Checklist

Based on the OWASP Top 10 and Microsoft's ASP.NET Core security documentation. Rate each finding **Critical**, **High**, **Medium** or **Low**, and cite file and line.

## 1. Broken access control (OWASP A01)

- Every controller/action that exposes non-public data has `[Authorize]` (and a role/policy where needed); public endpoints are deliberate (e.g. login, `HEAD /api/heartbeat`).
- GraphQL (`/api/graphql`) and the SignalR hub (`/api/health-hub`) require authorization.
- Users cannot read or change other users' records by guessing IDs.
- Server-side checks never rely on the Angular route guard (`auth.guard.ts`); it is only UX.

## 2. Cryptographic failures and secrets (A02)

- No real secrets in committed files. `appsettings.json` holds only `override-this` placeholders (SQL, Cosmos, Mongo, JWT, Blob); real values come from user-secrets, environment variables or Key Vault.
- Search the diff for connection strings, keys, tokens, passwords and private keys.
- HTTPS everywhere: `UseHttpsRedirection`, HSTS in production.
- Passwords are handled only by ASP.NET Identity; never hand-rolled hashing.

## 3. Injection (A03)

- SQL: EF Core LINQ or parameterized queries only. `FromSqlRaw` / string-concatenated SQL is a finding; use `FromSqlInterpolated` or parameters.
- MongoDB and Cosmos DB: filters built from typed builders or parameters, never from raw user strings.
- GraphQL: query depth/complexity limits, no sensitive fields exposed.
- Angular: no `innerHTML`, `bypassSecurityTrust*` or `eval` with user data.
- No user input in file paths, blob names or shell commands without validation.

## 4. Insecure design and validation (A04)

- Inputs validated on the server (data annotations / explicit checks that throw `BadRequestException`); length and range limits on strings and numbers.
- Request models do not allow over-posting (bind to ViewModels, not entities).
- Rate limiting or lockout on login and expensive endpoints.

## 5. Security misconfiguration (A05)

- CORS: the `AngularPolicy` origins (`AllowedCORS`) are explicit; no `AllowAnyOrigin` together with credentials.
- Detailed errors and developer exception pages are off in production; the global handler returns only `{ error }` with no stack traces.
- Swagger, GraphQL IDE and health details are not exposed publicly in production without auth.
- Security headers considered (`X-Content-Type-Options`, `Content-Security-Policy`, `X-Frame-Options`).

## 6. Vulnerable and outdated components (A06)

- Check new or updated NuGet and npm packages for known vulnerabilities (`dotnet list package --vulnerable`, `npm audit`).
- No unpinned or unmaintained packages added without a reason.

## 7. Authentication failures (A07)

- JWT: signature, issuer, audience and lifetime are validated; signing key is long, random and not hardcoded; token lifetime is short.
- Tokens are not logged and not placed in URLs.
- Identity password/lockout policies are not weakened.

## 8. Logging and monitoring (A09)

- Security-relevant events (failed logins, access denied) are logged.
- Logs (Serilog, `LogEvents` table) never contain passwords, tokens, keys or full personal data.
- No log forging: user input is passed as template arguments, not concatenated.

## 9. SSRF and external calls (A10)

- Outbound HTTP (weather lookup, advisor agent, etc.) uses configured base addresses; user input never decides the host. Timeouts are set and responses are validated.
- API keys for external services (e.g. Anthropic) come from configuration, never from code or the frontend.

## Output format

```
[High] WindPowerSystemV5.Server/Controllers/XController.cs:27
Issue: <vulnerability and how it can be exploited>
Fix: <concrete remediation>
```

End with a summary table of findings by severity, or state explicitly that no issues were found in the reviewed scope.

## Sources

- OWASP Top 10: https://owasp.org/www-project-top-ten/
- OWASP Cheat Sheet Series: https://cheatsheetseries.owasp.org/
- ASP.NET Core security docs: https://learn.microsoft.com/aspnet/core/security/
