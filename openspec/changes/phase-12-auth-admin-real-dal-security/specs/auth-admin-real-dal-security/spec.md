# Auth Admin Real DAL Security

## ADDED Requirements

### Requirement: Admin account data is DAL-backed
The system SHALL persist admin account data through BLL services and DAL repositories instead of relying only on in-memory stubs.

#### Scenario: Admin views accounts
- **WHEN** an Admin opens account management
- **THEN** `FrmUserManagement` SHALL call BLL only
- **AND** BLL SHALL read accounts through DAL
- **AND** the UI SHALL show username, full name, role, and active status
- **AND** password hashes SHALL NOT be shown

#### Scenario: Admin creates an account
- **WHEN** an Admin creates a valid account
- **THEN** BLL SHALL validate the input
- **AND** DAL SHALL persist the account using parameterized queries
- **AND** the password SHALL be stored hashed, not plaintext

#### Scenario: Deactivated user cannot log in
- **WHEN** a user account is inactive
- **THEN** authentication SHALL fail
- **AND** the UI SHALL receive a `ServiceResult<UserDto>` failure message

### Requirement: Role permissions are loaded through BLL
The system SHALL keep role permission lookup in BLL and DAL, never in WinForms SQL or direct DAL calls.

#### Scenario: Main shell renders navigation
- **WHEN** a user logs in
- **THEN** `FrmMain` SHALL ask BLL for accessible features or permission checks
- **AND** `FrmMain` SHALL NOT query DAL directly
- **AND** unauthorized screens SHALL remain hidden or blocked

#### Scenario: Admin views permission matrix
- **WHEN** an Admin opens role/permission management
- **THEN** BLL SHALL provide the role permission matrix
- **AND** the UI SHALL display Vietnamese screen labels clearly

### Requirement: Authentication uses password hashing for real accounts
The system SHALL verify real account passwords using a hashing helper or equivalent BLL-owned mechanism.

#### Scenario: User logs in with valid password
- **WHEN** the username is active and the password verifies against the stored hash
- **THEN** authentication SHALL return a successful `ServiceResult<UserDto>`
- **AND** the returned DTO SHALL include the correct role

#### Scenario: User logs in with invalid password
- **WHEN** the password does not verify
- **THEN** authentication SHALL fail
- **AND** no password hash SHALL be exposed to WinForms

### Requirement: Audit logs are DAL-backed and filterable
The system SHALL persist and read audit logs through BLL and DAL.

#### Scenario: Admin views audit logs
- **WHEN** an Admin opens audit logs
- **THEN** `FrmAuditLog` SHALL call `AuditLogService`
- **AND** the service SHALL read through DAL
- **AND** the grid SHALL show time, user, action, entity, and description

#### Scenario: Admin filters audit logs
- **WHEN** an Admin selects a date range or enters keyword/user/action text
- **THEN** the service SHALL return matching logs only
- **AND** DAL SHALL use parameters for filter values

#### Scenario: Important auth/admin event occurs
- **WHEN** login, account creation, account update, deactivation, or unauthorized navigation is handled in BLL where feasible
- **THEN** the system SHALL create an audit log entry

### Requirement: Architecture boundary remains strict
The auth/admin implementation SHALL preserve the required three-layer architecture.

#### Scenario: Source is reviewed
- **WHEN** source code is searched
- **THEN** WinForms SHALL not reference DAL
- **AND** WinForms SHALL not contain SQL
- **AND** DAL SHALL not reference WinForms
- **AND** repositories SHALL not show MessageBox
- **AND** user input in DAL queries SHALL use parameters

### Requirement: Password reset and email remain out of scope
The system SHALL not implement password reset, email delivery, or a full claims framework in this phase.

#### Scenario: Scope is reviewed
- **WHEN** Phase 12 is reviewed
- **THEN** password reset/email and complex claims framework SHALL be documented as deferred work
- **AND** the phase SHALL focus on persisted auth/admin DAL, hashing, permissions, audit logs, and tests
