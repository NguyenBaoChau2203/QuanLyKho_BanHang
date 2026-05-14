# Auth Admin Specification Delta

## ADDED Requirements

### Requirement: Real account registration is Admin-managed
The system SHALL support real account creation through Admin-managed account workflows only.

#### Scenario: Admin creates a real account
- **GIVEN** an Admin enters a unique username, full name, supported role, active status, and valid initial password
- **WHEN** the account is saved
- **THEN** BLL SHALL validate the input
- **AND** DAL SHALL persist the account using parameterized queries
- **AND** the password SHALL be stored as a hash, not plaintext
- **AND** an audit log entry SHALL be recorded

#### Scenario: Public self-registration remains disabled
- **WHEN** a user chooses any register action from the login flow
- **THEN** the system SHALL NOT create a persisted account directly from that unauthenticated action
- **AND** the UI SHALL explain that account creation is handled by Admin unless a later OpenSpec change approves public registration

### Requirement: Forgot password protects account existence
The system SHALL provide a forgot-password request behavior that does not reveal whether a username exists.

#### Scenario: User submits forgot-password request
- **WHEN** a user submits a username through forgot-password flow
- **THEN** the UI SHALL show a generic response regardless of whether the account exists
- **AND** BLL SHALL NOT expose account existence to WinForms through the response message
- **AND** BLL SHALL record an audit or recovery event where feasible

#### Scenario: Inactive account requests recovery
- **GIVEN** a username belongs to an inactive account
- **WHEN** forgot-password is requested
- **THEN** the UI response SHALL remain generic
- **AND** the account SHALL NOT receive a successful reset path without Admin review

### Requirement: Admin can reset account passwords
The system SHALL allow Admin users to reset passwords for real accounts through BLL services.

#### Scenario: Admin resets an active user's password
- **GIVEN** an Admin selects an active account
- **WHEN** the Admin resets the password
- **THEN** BLL SHALL verify Admin permission
- **AND** BLL SHALL generate or accept a valid temporary password
- **AND** BLL SHALL store only the hashed password through DAL
- **AND** password hash or plaintext password SHALL NOT be stored in audit log descriptions
- **AND** an audit log entry SHALL be recorded

#### Scenario: Reset password requires first-login change when enabled
- **GIVEN** the approved schema supports a must-change-password flag
- **WHEN** a user logs in with a reset temporary password
- **THEN** the system SHALL require the user to set a new password before normal navigation
- **AND** BLL SHALL clear the flag only after the new password is validated and hashed

### Requirement: Authentication uses password hashing
The system SHALL hash and verify real account passwords through BLL-owned security behavior.

#### Scenario: User logs in with a valid password
- **GIVEN** the user account is active
- **AND** the password verifies against the stored hash
- **WHEN** authentication is requested
- **THEN** authentication SHALL return a successful `ServiceResult<UserDto>`
- **AND** the returned DTO SHALL include safe user identity and role data only

#### Scenario: User logs in with invalid credentials
- **WHEN** authentication fails because the username is unknown, the account is inactive, or the password is invalid
- **THEN** authentication SHALL fail with a generic message
- **AND** no password hash SHALL be exposed to WinForms

#### Scenario: Account DTOs are safe for UI
- **WHEN** Admin account management loads users
- **THEN** returned DTOs SHALL NOT include password hashes, salts, plaintext passwords, or reset secrets

### Requirement: Account role and audit data are persisted through DAL
The system SHALL persist account, role/permission, and audit behavior through BLL services and DAL repositories.

#### Scenario: Role permissions are loaded for navigation
- **WHEN** a user logs in
- **THEN** WinForms SHALL ask BLL for accessible features or permission checks
- **AND** WinForms SHALL NOT query DAL directly
- **AND** unauthorized screens SHALL remain hidden or blocked

#### Scenario: Admin views audit logs
- **WHEN** Admin opens the audit log viewer
- **THEN** WinForms SHALL call `AuditLogService`
- **AND** `AuditLogService` SHALL read audit data through DAL
- **AND** filtering by date range, user/action/entity, or keyword SHALL use parameterized queries

#### Scenario: Important auth or admin event occurs
- **WHEN** login, forgot-password request, account creation, account update, deactivation, role change, password reset, or unauthorized access is handled
- **THEN** the system SHALL record an audit event where feasible

### Requirement: Auth admin implementation preserves architecture and security boundaries
The auth/admin implementation SHALL preserve the required project architecture and security rules.

#### Scenario: Source is reviewed
- **WHEN** source code is searched
- **THEN** WinForms SHALL not reference DAL
- **AND** WinForms SHALL not contain SQL
- **AND** DAL SHALL not reference WinForms
- **AND** repositories SHALL not show MessageBox
- **AND** DAL queries that use input values SHALL use parameters

#### Scenario: Secrets and passwords are reviewed
- **WHEN** repository files are reviewed
- **THEN** API keys, external secrets, plaintext real passwords, password hashes in UI DTOs, and reset secrets SHALL NOT be committed as application configuration

### Requirement: Unrelated optional features remain out of scope
The system SHALL not include unrelated optional features in this auth/admin recovery change.

#### Scenario: Scope is reviewed
- **WHEN** this change is reviewed
- **THEN** online AI SHALL remain out of scope
- **AND** print invoice SHALL remain out of scope
- **AND** Excel export SHALL remain out of scope
- **AND** unrelated UI redesign SHALL remain out of scope
