## 1. OpenSpec and Coordination

- [x] 1.1 Validate this OpenSpec change before implementation.
- [ ] 1.2 Châu reviews and approves any schema, seed, DTO, or public service contract changes.
- [ ] 1.3 Keep Phase 12 branches separate from `main`.

## 2. Dũ - Identity DAL and Account Backend

- [ ] 2.1 Review `database/schema.sql`, `database/seed.sql`, Phase 11 DTOs, and Phase 11 BLL services.
- [ ] 2.2 Add or update DAL repositories for `Users` and `Roles` using parameterized ADO.NET.
- [ ] 2.3 Add permission persistence through existing tables or proposed `Permissions`/`RolePermissions` tables if approved by Châu.
- [ ] 2.4 Update `UserAccountService` to use DAL-backed account CRUD while preserving `ServiceResult<T>` behavior.
- [ ] 2.5 Add validation/tests for duplicate usernames, invalid roles, deactivated accounts, and account CRUD.

## 3. Hùng - Audit DAL, Auth Tests, and Integration Hardening

- [ ] 3.1 Review Phase 11 `AuditLogService`, `AuthService`, `PermissionService`, and Admin UI contracts.
- [ ] 3.2 Add DAL repository for `AuditLogs` with date range and keyword filtering.
- [ ] 3.3 Update `AuditLogService` to use DAL-backed reads and BLL-safe audit writer methods.
- [ ] 3.4 Add tests for audit filtering, login success/failure audit entries, and admin account actions where feasible.
- [ ] 3.5 Help verify auth/admin behavior after Dũ wires account persistence.

## 4. Châu - Contract and UI Integration

- [ ] 4.1 Review Dũ/Hùng changes for contract safety and architecture boundaries.
- [ ] 4.2 Keep `FrmLogin`, `FrmMain`, and Admin forms calling BLL only.
- [ ] 4.3 Verify Vietnamese labels and messages remain demo-friendly.
- [ ] 4.4 Update docs or PR description for any public DTO/service/schema change.

## 5. Password Hashing

- [ ] 5.1 Add a BLL/internal password hashing helper or equivalent minimal service.
- [ ] 5.2 Store hashes for real accounts and never return hashes to WinForms DTOs.
- [ ] 5.3 Preserve deterministic demo login accounts for final presentation.
- [ ] 5.4 Add tests for successful verification, failed verification, and non-leaking account DTOs.

## 6. Validation

- [ ] 6.1 Run OpenSpec validation.
- [ ] 6.2 Run solution build.
- [ ] 6.3 Run solution tests with `--no-build --no-restore`.
- [ ] 6.4 Verify WinForms has no DAL references and no SQL.
- [ ] 6.5 Verify DAL queries use parameters for user input.
