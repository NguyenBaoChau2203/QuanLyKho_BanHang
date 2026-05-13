# Master Data UI

## ADDED Requirements

### Requirement: Master data screens provide searchable CRUD-style browsing
The system MUST provide polished WinForms screens for products, categories, suppliers, and customers with a search/filter strip, a data grid, an edit area, and standard actions for Add, Edit, Save, Cancel, Deactivate, and Refresh.

#### Scenario: User opens a master data screen
- **WHEN** the user opens `FrmProduct`, `FrmCategory`, `FrmSupplier`, or `FrmCustomer` from `FrmMain`
- **THEN** the screen shows Vietnamese labels, a searchable list, and an edit section
- **AND** the screen remains readable and resizable using standard WinForms controls

#### Scenario: User searches and edits master data
- **WHEN** the user types a keyword into the search box
- **THEN** the grid filters the visible rows by the current master data fields
- **AND** the selected row populates the edit panel for review or editing

#### Scenario: Service data is unavailable
- **WHEN** the related BLL service returns an empty list or failure message
- **THEN** the screen falls back to deterministic stub data so the UI remains demoable
- **AND** an empty or error state message is shown to the user

### Requirement: Master data UI must not use DAL or SQL directly
The system MUST keep all master data UI logic inside WinForms and call BLL service contracts only.

#### Scenario: Screen loads data
- **WHEN** a master data form refreshes its content
- **THEN** it calls `ProductService`, `CategoryService`, `SupplierService`, or `CustomerService`
- **AND** it does not reference DAL classes or SQL text
