# Proposal: Auth Admin Real Accounts Recovery

## Why

Phase 11 added demo-safe Admin screens for accounts, roles, permissions, and audit logs. Phase 12 already sketches real DAL-backed auth/admin security, but it intentionally defers password reset and does not make the account registration decision explicit.

This optional extension defines the real-account path for the final project without changing application code yet. It focuses on whether users can register, how forgotten passwords are recovered, how passwords are stored, and how account/role/audit data is persisted safely through the existing three-layer architecture.

## What Changes

- Decide that real account registration is Admin-managed only:
  - Admin creates and activates accounts.
  - The login screen does not support open public self-registration.
  - Any visible register action must clearly say account creation is handled by Admin unless a later OpenSpec change approves public registration.
- Add a forgot-password flow that does not reveal whether an account exists.
- Add an Admin reset-password flow for active accounts.
- Require password hashing for all real persisted accounts and reset passwords.
- Persist account, role, permission, recovery, and audit behavior through BLL services and DAL repositories.
- Define security acceptance criteria for authentication, reset, role checks, audit logging, and architecture boundaries.

## Relationship To Existing Changes

- Builds on `phase-11-admin-user-permission-audit`, which added demo Admin UI and BLL stubs.
- Overlaps with `phase-12-auth-admin-real-dal-security` for account/role/audit persistence and password hashing.
- Adds a stricter registration decision and a recovery/reset flow that Phase 12 explicitly deferred.
- If approved, the team should either merge this scope into Phase 12 or treat this change as the replacement auth/admin security plan. Do not implement both independently with conflicting contracts.

## Scope

Included:

- Admin-created real accounts.
- Forgot-password request behavior from the login flow.
- Admin reset-password behavior.
- Optional first-login password-change requirement after reset.
- Password hashing and verification owned by BLL/internal security helpers.
- DAL-backed account, role/permission, recovery request if needed, and audit log persistence.
- Parameterized ADO.NET queries for all user input.
- Tests and validation for security-critical behavior.

Out of scope:

- Online AI or DeepSeek/OpenAI integration.
- Print invoice.
- Excel export.
- Unrelated UI redesign or restyling.
- Public internet self-registration.
- Email, SMS, OTP delivery, MFA, and external identity providers.
- Complex claims/policy framework beyond the current role/permission model.
- Inventory, sales, reports, assistant, or dashboard business logic unrelated to auth/admin.

## Success Criteria

- OpenSpec validates for `auth-admin-real-accounts-recovery`.
- No application code is changed during proposal-only work.
- The approved implementation preserves `WinForms -> BLL -> DAL -> DTO`.
- Existing demo accounts can still be used after migration to hashed passwords:
  - `admin/admin123`
  - `manager/123456`
  - `du/123456`
  - `hung/123456`
- New and reset passwords are stored hashed, never plaintext.
- Admin account creation, role lookup, permission checks, password reset, and audit log reads use BLL services and DAL repositories.
- WinForms contains no SQL and does not reference DAL.
- DAL uses parameters for every value derived from user input.
- Forgot-password behavior avoids account enumeration.
