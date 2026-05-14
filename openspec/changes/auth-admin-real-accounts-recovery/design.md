# Design: Auth Admin Real Accounts Recovery

## Architecture

The implementation MUST preserve the existing dependency direction:

```text
WinForms -> BLL -> DAL -> DTO
```

WinForms may show login, forgot-password, and Admin account screens, but it calls BLL services only. BLL owns validation, password hashing orchestration, role checks, recovery decisions, and audit writing. DAL owns ADO.NET queries, mapping, and transactions. DTOs remain data-only and must not contain password hashing logic.

## Registration Decision

Real account registration is Admin-managed only for this project.

The login screen must not create real accounts directly. If a register action remains visible, it should route to a clear message such as "contact Admin to create an account" or a non-persisted request flow approved by this change. This keeps the classroom/demo app from creating uncontrolled accounts and keeps role assignment under Admin ownership.

Admin-created account behavior:

- Admin enters username, full name, role, active status, and initial password.
- BLL validates unique username, supported role, password rules, and active status.
- BLL hashes the password before DAL persistence.
- DAL stores only the password hash and account metadata.
- Audit logging records account creation and role assignment changes.

## Forgot Password And Admin Reset

Forgot-password behavior from login:

- User enters username.
- UI always shows a generic response that does not reveal whether the username exists.
- BLL records an audit event and, if a recovery request table is approved, stores a pending recovery request for active accounts.
- No email, SMS, OTP, or external delivery is required in this extension.

Admin reset behavior:

- Admin selects an active account and chooses reset password.
- BLL validates Admin permission and target account state.
- BLL generates or accepts a temporary password, hashes it, and persists the new hash.
- BLL sets a `MustChangePassword`-style flag if the schema/DTO change is approved.
- Audit logging records the reset without storing the plaintext temporary password.

First-login password change, if approved:

- A user logging in with a reset password is required to set a new password before normal navigation.
- BLL verifies the temporary password, validates the new password, stores a fresh hash, clears the reset flag, and records an audit event.

## Password Hashing

Password hashing is owned by BLL or an internal security helper used by BLL. WinForms never hashes, verifies, displays, or stores password hashes.

Minimum hashing design:

- Use .NET cryptography APIs such as PBKDF2 via `Rfc2898DeriveBytes` with a per-password random salt.
- Store a versioned hash format that includes algorithm, iteration count, salt, and hash data.
- Use a comparison method that avoids leaking password material through string output or logs.
- Support future hash-version migration without breaking existing accounts.
- Migrate seed/demo account passwords to hashes before claiming this extension is implemented.

Password policy can stay simple for the final project, but BLL should reject empty passwords, whitespace-only passwords, and passwords shorter than the agreed minimum length.

## Persistence

Existing schema already includes:

- `Roles`
- `Users`
- `AuditLogs`

This change MAY add or refine fields/tables if approved:

- `Users.MustChangePassword`
- `Users.UpdatedAt`
- `Users.LastLoginAt`
- `PasswordRecoveryRequests`
- `Permissions`
- `RolePermissions`

Any schema change must be documented in this OpenSpec change before code is implemented. DAL repositories must use parameterized ADO.NET and must not show UI dialogs or depend on WinForms.

Recommended repository responsibilities:

- `UserRepository`: load by username/id, create, update profile/role/status, update password hash, mark password-change requirement.
- `RoleRepository`: load roles and role names/ids.
- `PermissionRepository`: load role feature permissions if permissions move from deterministic BLL matrix to database.
- `AuditLogRepository`: write and query audit events by date range, user/action/entity, and keyword.
- `PasswordRecoveryRepository`: create and resolve recovery requests if the team chooses to persist requests.

## BLL Services

Auth/admin BLL services should keep public responses in `ServiceResult<T>` where practical.

Expected responsibilities:

- `AuthService`: authenticate active users, verify password hashes, handle reset-password login state, return safe `UserDto`.
- `UserAccountService`: create/update/deactivate accounts, reset passwords, enforce role and password validation.
- `PermissionService`: answer role-to-feature access checks and provide Admin permission matrix.
- `AuditLogService`: write important auth/admin events and query logs for Admin viewer.
- Optional `PasswordRecoveryService`: accept forgot-password requests and expose pending requests to Admin if a real request queue is approved.

No service should return password hashes or plaintext passwords to WinForms DTOs. If a temporary password must be shown after Admin reset, it should be shown once and never stored as plaintext.

## Security Acceptance Criteria

Implementation will be accepted only if:

- Login failure messages are generic enough to avoid revealing whether username or password was wrong.
- Forgot-password response does not reveal whether the account exists.
- Deactivated accounts cannot log in or request successful resets.
- New/reset passwords are persisted only as hashes.
- Account grids and DTOs do not expose password hashes.
- Admin-only actions require Admin permission checks in BLL.
- Important events are audited: login success/failure where feasible, account create/update/deactivate, role change, password reset, forgot-password request, unauthorized access.
- DAL uses parameters for all user input.
- WinForms has no direct DAL reference and no SQL.
- No API keys, secrets, or plaintext passwords are committed as source configuration.

## Validation Plan

- `npx --yes --package @fission-ai/openspec openspec validate auth-admin-real-accounts-recovery`
- During implementation only:
  - `dotnet build QuanLyKhoBanHang.sln`
  - `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`
  - Search WinForms for DAL references and SQL strings.
  - Review DAL auth/admin queries for parameterized input.

## Deferred Work

- Online AI integration.
- Print invoice.
- Excel export.
- Unrelated UI redesign.
- Email/SMS/OTP recovery delivery.
- MFA and external identity providers.
- Full claims-based authorization framework.
