# Admin User Permission Audit

## ADDED Requirements

### Requirement: Demo authentication supports multiple roles
The system SHALL authenticate deterministic demo accounts and return the correct `UserDto.Role` for each account.

#### Scenario: Admin logs in
- **WHEN** the user logs in with username `admin` and password `admin123`
- **THEN** authentication SHALL succeed
- **AND** the returned role SHALL be `Admin`

#### Scenario: Manager logs in
- **WHEN** the user logs in with username `manager` and password `123456`
- **THEN** authentication SHALL succeed
- **AND** the returned role SHALL be `Manager`

#### Scenario: Warehouse staff logs in
- **WHEN** the user logs in with username `du` and password `123456`
- **THEN** authentication SHALL succeed
- **AND** the returned role SHALL be `WarehouseStaff`

#### Scenario: Sales staff logs in
- **WHEN** the user logs in with username `hung` and password `123456`
- **THEN** authentication SHALL succeed
- **AND** the returned role SHALL be `SalesStaff`

### Requirement: Main shell navigation is role based
The system SHALL render sidebar and quick-action navigation according to the logged-in user's role and SHALL prevent unauthorized screen opening from UI navigation.

#### Scenario: Admin sees all navigation
- **WHEN** an Admin opens the main shell
- **THEN** the sidebar SHALL include all business screens
- **AND** the Admin section SHALL include `Tài khoản`, `Phân quyền`, and `Nhật ký hệ thống`

#### Scenario: Manager sees management navigation
- **WHEN** a Manager opens the main shell
- **THEN** the sidebar SHALL include dashboard, products, categories, suppliers, customers, inventory/stocktake overview, reports, and assistant AI
- **AND** the sidebar SHALL NOT include account management, role/permission management, or audit log viewer

#### Scenario: WarehouseStaff sees warehouse navigation
- **WHEN** a WarehouseStaff user opens the main shell
- **THEN** the sidebar SHALL include products, categories, suppliers, purchase receipt, inventory, and stocktake
- **AND** the sidebar SHALL NOT include sales invoice, reports, or Admin-only screens

#### Scenario: SalesStaff sees sales navigation
- **WHEN** a SalesStaff user opens the main shell
- **THEN** the sidebar SHALL include customers, sales invoice, and basic product/inventory lookup when available
- **AND** the sidebar SHALL NOT include purchase receipt, stocktake, reports, or Admin-only screens

#### Scenario: Unauthorized navigation is blocked
- **WHEN** a user attempts to open a screen that their role cannot access through shell navigation helpers
- **THEN** the system SHALL block the navigation
- **AND** the UI SHALL show a clear Vietnamese authorization message

### Requirement: Admin can manage demo accounts in stub mode
The system SHALL provide an Admin-only account management screen backed by BLL services.

#### Scenario: Admin opens account management
- **WHEN** an Admin opens `FrmUserManagement`
- **THEN** the screen SHALL show demo accounts with username, full name, role, and active status
- **AND** the screen SHALL not show real password hashes or sensitive values

#### Scenario: Admin edits demo accounts
- **WHEN** an Admin creates, edits, or deactivates an account in the screen
- **THEN** the action SHALL be handled by `UserAccountService`
- **AND** the UI SHALL refresh with deterministic stub-mode data
- **AND** no WinForms code SHALL call DAL or contain SQL

### Requirement: Admin can view role permissions
The system SHALL provide an Admin-only role/permission overview backed by BLL services.

#### Scenario: Admin opens role permission matrix
- **WHEN** an Admin opens `FrmRolePermission`
- **THEN** the screen SHALL show which roles can access which screens
- **AND** the labels SHALL be professional Vietnamese labels that are not cut off

### Requirement: Admin can view audit logs
The system SHALL provide an Admin-only readonly audit log viewer backed by BLL services.

#### Scenario: Admin opens audit log viewer
- **WHEN** an Admin opens `FrmAuditLog`
- **THEN** the screen SHALL show deterministic demo audit logs
- **AND** the grid SHALL include time, user, action, entity, and description columns

#### Scenario: Admin filters audit logs
- **WHEN** an Admin selects a date range or enters keyword/user/action text
- **THEN** the audit log list SHALL filter accordingly
- **AND** the logs SHALL remain readonly

### Requirement: Admin implementation preserves project architecture
The system SHALL preserve the WinForms -> BLL -> DAL -> DTO architecture for admin features.

#### Scenario: Admin implementation is reviewed
- **WHEN** source code is searched
- **THEN** WinForms SHALL not reference DAL
- **AND** WinForms SHALL not contain SQL
- **AND** Admin UI SHALL call BLL services only
- **AND** DTOs SHALL remain data-only
- **AND** real user/audit repository implementation SHALL remain out of scope unless a later OpenSpec change adds it
