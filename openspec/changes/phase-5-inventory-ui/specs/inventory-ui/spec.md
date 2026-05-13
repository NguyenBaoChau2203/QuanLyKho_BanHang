# Inventory UI

## ADDED Requirements

### Requirement: Inventory screens support receipt, stock, and stocktake workflows
The system MUST provide WinForms screens for purchase receipts, current stock, low-stock viewing, and stocktake with product search, line-item grids, quantity input, summary panels, and clear validation messaging.

#### Scenario: User opens inventory screens
- **WHEN** the user opens `FrmPurchaseReceipt`, `FrmInventory`, or `FrmStocktake` from `FrmMain`
- **THEN** the screen shows the expected workflow sections in Vietnamese
- **AND** the screen uses standard WinForms controls with responsive layout

#### Scenario: User reviews or enters inventory lines
- **WHEN** the user searches for a product and adds it to a receipt or stocktake grid
- **THEN** the line item grid shows the item details and updates summary values
- **AND** validation messages appear when input is missing or invalid

#### Scenario: Backend data is not yet available
- **WHEN** the related BLL service returns empty data or a failure message
- **THEN** the screen uses deterministic stub data and safe fallback states
- **AND** the screen remains demoable without direct DAL access

### Requirement: Inventory UI must not use DAL or SQL directly
The system MUST keep inventory screens dependent on BLL service contracts only.

#### Scenario: Screen loads stock data
- **WHEN** an inventory screen refreshes its view
- **THEN** it calls `ProductService`, `PurchaseService`, `InventoryService`, or `StocktakeService`
- **AND** it does not include SQL or DAL references in WinForms code
