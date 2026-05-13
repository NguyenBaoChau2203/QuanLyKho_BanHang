# Phase 11 Admin User Permission Audit

## Why

The demo currently authenticates only the admin account and exposes the same shell navigation to every user. The project needs a clearer admin story for the final presentation: Admin can manage demo accounts, inspect role access, and review system audit logs, while Manager, WarehouseStaff, and SalesStaff see only the screens appropriate to their responsibility.

## What Changes

- Improve demo authentication with deterministic role accounts:
  - `admin/admin123` -> Admin
  - `manager/123456` -> Manager
  - `du/123456` -> WarehouseStaff
  - `hung/123456` -> SalesStaff
- Add Admin-only account management UI for viewing, creating, editing, and deactivating demo accounts in BLL stub mode.
- Add Admin-only role/permission overview UI that shows which role can access each screen.
- Add Admin-only readonly audit log viewer with date and keyword filtering.
- Add role-based sidebar and quick-action visibility in `FrmMain`.
- Show the current user's full name and role in the main shell.
- Keep implementation demo-safe through DTOs and BLL services; no real user/audit DAL work is required in this phase.

## Non-Goals

- Do not implement real database repositories for users, permissions, or audit logs.
- Do not implement password reset, email, complex claims, or production-grade password hashing.
- Do not modify Dũ/Hùng inventory, sales, or report business logic.
- Do not add third-party UI frameworks.
- Do not store, show, or commit sensitive keys or secrets.

## Impact

- Adds new DTO/BLL contracts for demo-safe admin features.
- Updates `AuthService`, login, and main shell to pass the full `UserDto` and enforce role-based UI access.
- Adds three WinForms screens under Admin scope while preserving WinForms -> BLL -> DTO boundaries.
- Leaves DAL/database schema unchanged for this phase, even though existing schema already has `Users`, `Roles`, and `AuditLogs`.

## Success Criteria

- OpenSpec validates for `phase-11-admin-user-permission-audit`.
- The solution builds and tests pass.
- Each demo login opens the app with the correct role.
- Admin sees all menus plus `Tài khoản`, `Phân quyền`, and `Nhật ký hệ thống`.
- Manager, WarehouseStaff, and SalesStaff do not see Admin-only screens.
- Unauthorized screens cannot be opened from UI navigation or quick actions.
- Admin account management shows demo accounts and supports stub create/edit/deactivate behavior.
- Role/permission screen shows a clear permission matrix.
- Audit log screen shows deterministic demo logs and filters by date, keyword, user/action text.
- WinForms contains no SQL and does not reference DAL.
