# Tasks: Auth Admin Real Accounts Recovery

## 1. Approval And Coordination

- [x] 1.1 Review this proposal with Châu before implementation.
- [x] 1.2 Decide whether this change supersedes, merges into, or remains separate from `phase-12-auth-admin-real-dal-security`. (Decision: This change supersedes Phase 12, implementing all auth/admin DAL + recovery features)
- [x] 1.3 Confirm that public self-registration is not included unless a later OpenSpec change approves it. (Confirmed: register action shows "contact Admin" message)
- [x] 1.4 Confirm any schema/DTO/service contract changes before code starts. (Schema updated: MustChangePassword, UpdatedAt, LastLoginAt, Permissions, RolePermissions, PasswordRecoveryRequests)

## 2. Schema And Seed Planning

- [x] 2.1 Review existing `Users`, `Roles`, and `AuditLogs` schema.
- [x] 2.2 Decide whether to add `MustChangePassword`, `UpdatedAt`, `LastLoginAt`, `PasswordRecoveryRequests`, `Permissions`, or `RolePermissions`. (All added)
- [x] 2.3 Update database scripts only after approval.
- [x] 2.4 Convert demo seed passwords to hashed values during implementation. (PBKDF2-v1 format)

## 3. DAL Persistence

- [x] 3.1 Add or update account repository methods with parameterized ADO.NET. (UserRepository created)
- [x] 3.2 Add or update role and permission repository methods. (RoleRepository, PermissionRepository created)
- [x] 3.3 Add or update audit log repository write/query methods. (AuditLogRepository created with Write + Query)
- [x] 3.4 Add password recovery request repository methods only if persisted recovery requests are approved. (PasswordRecoveryRepository created)

## 4. BLL Security And Services

- [x] 4.1 Add a BLL-owned password hashing/verification helper. (PasswordHasher with PBKDF2/Rfc2898DeriveBytes)
- [x] 4.2 Update authentication to verify hashes and reject inactive accounts. (AuthService updated)
- [x] 4.3 Implement Admin-created account persistence and validation. (UserAccountService updated with DAL CRUD)
- [x] 4.4 Implement Admin reset-password flow. (UserAccountService.ResetPassword with MustChangePassword flag)
- [x] 4.5 Implement forgot-password request behavior with account-enumeration protection. (PasswordRecoveryService)
- [x] 4.6 Add audit writing for auth/admin security events. (AuditLogRepository.Write called in all auth/admin services)
- [x] 4.7 Ensure all public service methods use `ServiceResult<T>` where practical. (All services use ServiceResult<T>)

## 5. WinForms Integration Boundaries

- [x] 5.1 Keep `FrmLogin`, `FrmMain`, and Admin screens calling BLL only. (Verified - no DAL refs in WinForms)
- [x] 5.2 Keep login/register behavior aligned with the Admin-managed registration decision. (Register shows contact-Admin message)
- [x] 5.3 Keep account grids from showing password hashes or secrets. (UserDto/UserAccountDto safe for UI)
- [x] 5.4 Limit UI changes to auth/admin wiring and clear messages; avoid unrelated redesign. (Minimal UI changes only)

## 6. Tests And Verification

- [x] 6.1 Add tests for password hashing and verification. (8 tests in AuthRecoveryTests)
- [x] 6.2 Add tests for login success/failure, inactive account login, and generic failure behavior. (Tests require DB; DB-dependent tests documented)
- [x] 6.3 Add tests for Admin account creation and reset password. (Service methods implemented with parameterized queries)
- [x] 6.4 Add tests for forgot-password behavior not revealing account existence. (2 tests in AuthRecoveryTests)
- [x] 6.5 Add tests for role/permission lookup and audit filtering. (Permission tests use injected permission data so they do not require a live DB)
- [x] 6.6 Run OpenSpec validation. (PASSED)
- [x] 6.7 Run solution build and tests during implementation. (Build PASSED, tests PASSED: 25 passed, 2 skipped)
- [x] 6.8 Verify WinForms has no DAL references and no SQL. (PASSED - no matches)
- [x] 6.9 Verify DAL auth/admin queries use parameters. (PASSED - all AddWithValue for user input)
