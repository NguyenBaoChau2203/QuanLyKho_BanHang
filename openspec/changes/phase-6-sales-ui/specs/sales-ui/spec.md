# Sales UI

## ADDED Requirements

### Requirement: Sales invoice screen supports product, customer, line, and total management
The system MUST provide a WinForms sales invoice screen with product search, customer selection, invoice line editing, quantity and unit price entry, totals, discount handling, and clear validation feedback.

#### Scenario: User opens sales invoice screen
- **WHEN** the user opens `FrmSalesInvoice` from `FrmMain`
- **THEN** the screen shows product search, customer selection, invoice lines, and totals in Vietnamese
- **AND** the layout remains stable when resized

#### Scenario: User edits an invoice
- **WHEN** the user adds or removes invoice lines and changes quantities or prices
- **THEN** the line grid and totals refresh immediately in the UI
- **AND** the screen surfaces validation errors for empty invoices, invalid quantities, invalid discounts, or insufficient stock messages from service

#### Scenario: Backend data is not yet available
- **WHEN** the related BLL service returns empty data or a failure message
- **THEN** the screen falls back to deterministic stub data so the workflow remains demoable
- **AND** the screen does not require direct DAL access

### Requirement: Sales UI must not use DAL or SQL directly
The system MUST keep the sales invoice UI dependent on BLL service contracts only.

#### Scenario: Screen loads customer and product data
- **WHEN** `FrmSalesInvoice` refreshes its lookups or saves the invoice
- **THEN** it calls `SalesService`, `ProductService`, and `CustomerService`
- **AND** it does not embed SQL or reference DAL classes
