# Design: Phase 12 Auth Admin Real DAL Security

## Architecture

The implementation MUST preserve the dependency direction:

```text
WinForms -> BLL -> DAL -> DTO
```

WinForms continues to call only BLL services. DAL contains ADO.NET queries, mapping, and transactions. DTO remains data-only.

## Recommended Split

Two people is appropriate if the work is done with clear file ownership:

- Dũ handles identity persistence: user, role, permission repositories and account-service persistence.
- Hùng handles audit persistence and auth/admin behavior tests.
- Châu reviews database scripts, contracts, and UI integration.

One person is safer only if the deadline is very tight or the team wants a minimal version. The single-owner version should include Users/Roles/AuditLogs persistence and password hashing, while deferring editable permission persistence.

## Database Design

Existing tables already include:

- `Roles`
- `Users`
- `AuditLogs`

Phase 12 MAY add tables if permission persistence is required:

- `Permissions`
- `RolePermissions`

Any schema change MUST:

- use PascalCase columns
- keep SQL Server LocalDB compatibility
- use `DATETIME2` for timestamps
- preserve seed demo accounts
- avoid plaintext passwords for newly seeded real accounts

## Password Handling

BLL should expose password operations through a small internal helper or service, not through WinForms.

Minimum behavior:

- new passwords are hashed before storage
- authentication verifies the password through the hashing helper
- plaintext demo passwords may appear only as seed input or tests, not as persisted password output
- account screens never display password hashes

No password reset or email flow is included in this phase.

## DAL Rules

Repositories MUST:

- use ADO.NET only
- use parameters for all user input
- not show MessageBox
- not reference WinForms
- return DTO/domain data to BLL

## BLL Rules

Services MUST:

- keep public methods returning `ServiceResult<T>` where practical
- validate username, full name, role, active status, and password input
- orchestrate repository calls
- write audit logs for important auth/admin events where feasible
- avoid leaking password hashes to UI DTOs

## UI Rules

Existing Phase 11 screens should stay mostly stable:

- `FrmUserManagement`
- `FrmRolePermission`
- `FrmAuditLog`
- `FrmLogin`
- `FrmMain`

UI changes should be limited to wiring BLL results and clear Vietnamese messages.

## Validation Plan

- `npx --yes --package @fission-ai/openspec openspec validate phase-12-auth-admin-real-dal-security`
- `dotnet build QuanLyKhoBanHang.sln`
- `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`
- Search WinForms for DAL references.
- Search WinForms for SQL keywords.
- Review DAL queries for parameter usage.

## Deferred Work

- password reset and email delivery
- full claims/policy framework
- centralized session expiration
- admin approval workflows
- production secret management beyond not committing secrets
