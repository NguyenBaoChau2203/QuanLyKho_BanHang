# Phase 12 Auth Admin Real DAL Security

## Why

Phase 11 completed the demo-safe Admin UI and BLL stubs for accounts, permissions, and audit logs. The next step is to turn that foundation into real persisted backend behavior without breaking the WinForms -> BLL -> DAL -> DTO architecture.

This work is larger than one screen because it touches identity data, password handling, role permissions, audit logging, database scripts, services, and tests. It should be split between two backend contributors, with Châu owning the OpenSpec/schema review and final UI integration.

## What Changes

- Replace Phase 11 in-memory admin stubs with DAL-backed services where repository contracts are ready.
- Add or refine ADO.NET repositories for:
  - users
  - roles
  - permissions or role-permission mappings
  - audit logs
- Add password hashing for real stored accounts while preserving deterministic demo login data.
- Record security/admin actions into audit logs through BLL services.
- Keep Admin WinForms screens calling BLL only.
- Add focused tests for auth, account CRUD, permission lookup, audit filtering, and hashing behavior.

## Recommended Work Split

Use two implementers, but keep authentication/password decisions under one owner to avoid inconsistent security behavior.

### Dũ: Identity DAL and Account Backend

- Own DAL repositories for `Users`, `Roles`, and permission mappings.
- Implement parameterized ADO.NET queries for account CRUD and role/permission reads.
- Implement BLL account service persistence behind the Phase 11 `UserAccountService` style contract.
- Add tests for account validation, duplicate username handling, active/deactivated accounts, and role loading.

### Hùng: Audit DAL, Auth Tests, and Integration Hardening

- Own DAL repository for `AuditLogs`.
- Implement audit log querying with date range and keyword filters.
- Add BLL audit writer methods used by auth/admin services.
- Add tests for audit filtering, login success/failure audit events, and unauthorized access audit events where feasible.

### Châu: Owner/Reviewer

- Own OpenSpec, schema/seed review, contract review, and WinForms integration.
- Review any `database/schema.sql`, `database/seed.sql`, DTO, or public service signature changes before merge.
- Keep `FrmLogin`, `FrmMain`, and Admin forms stable unless UI integration is necessary.

## Non-Goals

- Do not implement password reset or email flows in this phase.
- Do not introduce a complex claims framework or third-party identity package.
- Do not rewrite unrelated inventory, sales, report, or assistant business logic.
- Do not store API keys or secrets.
- Do not push directly to `main`.

## Impact

- Existing Phase 11 Admin UI should remain usable while BLL moves from stub data to real DAL data.
- Database scripts may need controlled updates for permission persistence and hashed password seed data.
- Tests should grow around security-critical behavior.

## Success Criteria

- OpenSpec validates.
- Solution builds and tests pass.
- Existing demo logins still work:
  - `admin/admin123`
  - `manager/123456`
  - `du/123456`
  - `hung/123456`
- New or updated users persist through DAL-backed services.
- Deactivated users cannot log in.
- Passwords are not stored as plaintext for new real accounts.
- Role permissions load through BLL/DAL, not WinForms.
- Audit log viewer reads DAL-backed audit data and filters correctly.
- WinForms contains no SQL and does not reference DAL.
