# Integration Demo Polish

## ADDED Requirements

### Requirement: Demo-ready WinForms polish
The WinForms application SHALL present a consistent, readable, and responsive UI across the main business screens used in the demo.

#### Scenario: User opens major screens
- **WHEN** the user opens Login, Main shell, Dashboard, Master data, Customers, Purchase receipt, Inventory, Stocktake, Sales invoice, Reports, or Assistant
- **THEN** the screen SHALL use consistent spacing, labels, grid styling, and resize behavior
- **AND** empty or error states SHALL be visible and understandable

### Requirement: Contract-safe backend integration
The WinForms application SHALL call only BLL services that already exist and match the Phase 0 contract.

#### Scenario: Existing service is ready
- **WHEN** a screen needs data and the service implementation already exists with a compatible DTO and `ServiceResult<T>` contract
- **THEN** the screen SHALL use that service result directly
- **AND** no DAL access SHALL be added to WinForms

#### Scenario: Backend is unfinished
- **WHEN** a service is missing or returns empty/unready data
- **THEN** the screen SHALL keep deterministic fallback data or empty-state messaging
- **AND** the UI SHALL remain usable for demo purposes

### Requirement: Final demo checklist
The project SHALL keep a final demo checklist that reflects the intended showcase flow.

#### Scenario: Checklist review
- **WHEN** the team prepares the demo
- **THEN** the checklist SHALL include login, dashboard, master data, nhập kho, bán hàng, báo cáo, and assistant questions
- **AND** the checklist SHALL mention fallback behavior where backend data is not yet available