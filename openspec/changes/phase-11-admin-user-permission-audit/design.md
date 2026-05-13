# Design: Phase 11 Admin User Permission Audit

## Architecture

The implementation keeps the existing layered dependency direction:

```text
WinForms -> BLL -> DTO
BLL -> DTO
DAL remains out of scope for this phase
```

WinForms forms call BLL services only. Admin features use deterministic in-memory/stub BLL data so the demo remains stable without adding real repositories.

## DTO Contracts

Add data-only DTOs when needed:

- `UserAccountDto`
  - account identity, username, full name, role, active status, created timestamp
  - optional password input field for demo create/edit only, without hashing requirements in this phase
- `RolePermissionDto`
  - role, screen/feature key, Vietnamese display name, access flag, notes
- `AuditLogDto`
  - time, user, action, entity, description

DTOs remain property bags and do not contain validation or behavior.

## BLL Services

Add deterministic services:

- `UserAccountService`
  - returns demo accounts
  - validates basic username/full name/role input
  - supports create, update, and deactivate in stub mode
- `PermissionService`
  - owns the role-to-screen matrix used by both Admin overview and main-shell navigation
  - exposes permission checks such as whether a role can access a feature key
- `AuditLogService`
  - returns readonly demo audit logs
  - filters by date range and keyword/user/action text

`AuthService` will authenticate the four demo accounts and return a full `UserDto` with the correct `UserRole`.

## Role Model

Role access follows these demo rules:

- Admin: full access, including account management, role/permission management, and audit log viewer.
- Manager: dashboard, products/categories/suppliers/customers, inventory/stocktake overview, reports, assistant AI; no Admin-only screens.
- WarehouseStaff: products/categories/suppliers, purchase receipt, inventory, stocktake; no sales/report/Admin screens.
- SalesStaff: customers, sales invoice, and basic product/inventory lookup if available; no purchase/stocktake/report/Admin screens.

The main shell will build sidebar buttons from the permission service instead of showing every button unconditionally.

## WinForms UI

Add Admin screens:

- `FrmUserManagement`
  - grid columns: username, full name, role, active status
  - simple editor controls for demo create/edit/deactivate
- `FrmRolePermission`
  - readonly matrix or safe editable stub showing roles versus screens
- `FrmAuditLog`
  - readonly grid with time, user, action, entity, description
  - date range and keyword filters

Update existing forms:

- `FrmLogin`
  - keeps BLL-only authentication
  - passes the full `UserDto` into `FrmMain`
- `FrmMain`
  - shows current full name and role
  - renders sidebar and quick actions by role
  - blocks unauthorized navigation with a user-friendly message
  - keeps sidebar button rows fixed height so the assistant button does not stretch vertically

## Validation Plan

- Run `npx --yes --package @fission-ai/openspec openspec validate phase-11-admin-user-permission-audit`.
- Build with `dotnet build QuanLyKhoBanHang.sln`.
- Test with `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.
- Search WinForms source for DAL references and SQL strings.
- Confirm the WinForms project file does not reference `QuanLyKhoBanHang.DAL`.

## Deferred Work

Real DAL repositories for user accounts, permissions, and audit logs are intentionally deferred. A future phase may replace BLL stub data with parameterized ADO.NET repositories while keeping the same WinForms -> BLL contract.
