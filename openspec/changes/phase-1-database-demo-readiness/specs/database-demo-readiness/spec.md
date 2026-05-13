# Database Demo Readiness

## ADDED Requirements

### Requirement: Demo seed data supports login and dashboard flows
The system MUST keep `database/seed.sql` populated with demo accounts and operational data that allow login, dashboard display, and basic master data browsing immediately after running the scripts.

#### Scenario: Seeded demo data is ready for use
- **WHEN** the database scripts are executed in order
- **THEN** the seed includes login accounts for `admin`, `du`, and `hung`
- **AND** the seed includes categories, suppliers, customers, products, and stock history sufficient for demo browsing
- **AND** the seed includes at least one low-stock product example

### Requirement: Seed data is safe for later backend integration
The system MUST keep the demo-ready database scripts contract-preserving so later DAL/BLL work can integrate without renaming or removing public schema fields used by the existing contract.

#### Scenario: Schema contract remains intact
- **WHEN** the phase 1 database scripts are reviewed after the update
- **THEN** the existing table names, primary keys, foreign keys, business codes, and core contract columns remain available
- **AND** any schema improvement is additive or otherwise explicitly contract-preserving

### Requirement: Database documentation reflects demo readiness
The system MUST keep `database/README.md` aligned with the demo-ready seed expectations and demo account information.

#### Scenario: README explains demo setup
- **WHEN** a team member reads the database README
- **THEN** it describes the LocalDB connection string, script order, demo accounts, and demo password handling notes
