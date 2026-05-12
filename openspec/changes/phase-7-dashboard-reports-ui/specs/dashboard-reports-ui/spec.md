# Dashboard and Reports UI

## ADDED Requirements

### Requirement: Dashboard screen provides operational overview for inventory and sales managers
The system MUST provide a WinForms dashboard that shows revenue today, revenue this month, invoice count today, low-stock count, top products, low-stock products, and recent activity in Vietnamese.

#### Scenario: User opens dashboard
- **WHEN** the user opens `FrmDashboard` from `FrmMain`
- **THEN** the screen shows KPI cards, top products, low-stock products, and recent activity
- **AND** the layout remains stable when resized

#### Scenario: Service data is empty
- **WHEN** `DashboardService`, `ReportService`, or `InventoryService` returns an empty result
- **THEN** the dashboard uses deterministic stub data so the screen remains useful for demo
- **AND** the screen shows a clear empty or fallback state message when appropriate

#### Scenario: Service returns an error
- **WHEN** a service returns a failure result
- **THEN** the dashboard shows a clear error state without crashing
- **AND** the UI remains responsive

### Requirement: Reports screen supports date filtering and summary views
The system MUST provide a WinForms reports screen with from/to date filters, a refresh action, a revenue summary grid, a top products grid, a top customers grid, and a disabled or placeholder export action if export is not ready.

#### Scenario: User filters report data
- **WHEN** the user changes the date range and clicks refresh
- **THEN** the reports screen reloads revenue, product, and customer summaries from BLL services
- **AND** the grid contents update without requiring DAL access from WinForms

#### Scenario: Export is not implemented yet
- **WHEN** the user sees the export action
- **THEN** the export button is disabled or clearly marked as a placeholder
- **AND** the screen does not attempt to export real files

### Requirement: Dashboard and reports UI must remain presentation-only
The system MUST keep dashboard and reports screens dependent only on BLL service contracts and shared UI helpers.

#### Scenario: Screen loads data
- **WHEN** `FrmDashboard` or `FrmReport` refreshes data
- **THEN** it calls only `DashboardService`, `ReportService`, and `InventoryService`
- **AND** it does not contain SQL strings or reference DAL classes
