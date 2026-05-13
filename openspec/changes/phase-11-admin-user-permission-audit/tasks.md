## 1. OpenSpec Setup

- [x] 1.1 Create proposal, design, tasks, and spec artifacts for `phase-11-admin-user-permission-audit`.
- [x] 1.2 Validate the OpenSpec change before implementation.

## 2. DTO and BLL Contracts

- [x] 2.1 Add admin DTOs for user accounts, role permissions, and audit logs.
- [x] 2.2 Add deterministic BLL services for account management, permission matrix, and audit logs.
- [x] 2.3 Update `AuthService` to support `admin`, `manager`, `du`, and `hung` demo role logins.

## 3. Admin WinForms Screens

- [x] 3.1 Add `FrmUserManagement` with demo account grid and create/edit/deactivate stub behavior.
- [x] 3.2 Add `FrmRolePermission` with role/screen permission matrix.
- [x] 3.3 Add `FrmAuditLog` with readonly grid and date/keyword filters.

## 4. Main Shell and Authentication

- [x] 4.1 Update login flow to pass full `UserDto` into the main shell.
- [x] 4.2 Render sidebar and quick actions by role using BLL permission data.
- [x] 4.3 Show current user and role, block unauthorized UI navigation, and keep sidebar buttons fixed height.

## 5. Architecture Safety

- [x] 5.1 Verify WinForms does not reference DAL.
- [x] 5.2 Verify WinForms contains no SQL.
- [x] 5.3 Preserve existing inventory, sales, report, and assistant business logic.

## 6. Validation and Delivery

- [x] 6.1 Run OpenSpec validation.
- [x] 6.2 Run solution build.
- [x] 6.3 Run solution tests with `--no-build --no-restore`.
- [x] 6.4 Mark tasks complete after validation/build/test pass.
- [x] 6.5 Commit with `feat(admin): add account permissions and audit log UI`.
