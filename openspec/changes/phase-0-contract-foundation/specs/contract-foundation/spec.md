# Contract Foundation

## ADDED Requirements

### Requirement: Database contract is stable
The system MUST keep `database/schema.sql` as the shared contract for the core LocalDB schema supporting users, categories, products, suppliers, customers, purchase receipts, sales invoices, stock transactions, stocktakes, and audit logs.

#### Scenario: Schema supports all Phase 0 teams
- **WHEN** the team reads the database contract before implementation
- **THEN** the required tables, keys, constraints, and indexes are defined for parallel work

### Requirement: Seed data is deterministic
The system MUST keep `database/seed.sql` as deterministic demo data that supports login and basic inventory/sales/report demonstrations without manual inserts.

#### Scenario: Demo data is available immediately
- **WHEN** the database scripts are run in order
- **THEN** the seeded data includes users, categories, suppliers, customers, products, and starting stock history

### Requirement: DTOs are shared contracts
The system MUST treat DTOs in `src/QuanLyKhoBanHang.DTO/` as the shared data contract between WinForms and BLL.

#### Scenario: UI can bind to stable DTO shapes
- **WHEN** Châu builds forms against DTOs
- **THEN** property names remain stable enough that later backend implementation does not break the UI

### Requirement: Public BLL service contracts are stable
The system MUST expose stable public BLL service contracts for master data, inventory, sales, reports, and assistant workflows, excluding invoice printing in Phase 0.

#### Scenario: Service calls are predictable
- **WHEN** any team member invokes a public BLL method
- **THEN** the method name, parameters, and `ServiceResult<T>` return type are stable and documented

#### Scenario: Invoice printing is out of scope
- **WHEN** the team reviews Phase 0 service coverage
- **THEN** no `PrintInvoice` service method is defined or finalized in this change

### Requirement: Stub-compatible behavior exists
The system MUST provide stub-compatible behavior for public services so WinForms work can continue before DAL/BLL implementation is complete.

#### Scenario: UI development is unblocked
- **WHEN** a service is not backed by real DAL logic yet
- **THEN** it still returns valid DTO-shaped data or a deterministic validation failure through the same contract

### Requirement: Ownership and contract change rules are defined
The system MUST define ownership rules so Châu, Dũ, and Hùng can change only their approved files without breaking shared contracts.

#### Scenario: Contract change is controlled
- **WHEN** someone proposes a schema, DTO, or public service signature change
- **THEN** the change is treated as a contract change and requires Châu review before merge
