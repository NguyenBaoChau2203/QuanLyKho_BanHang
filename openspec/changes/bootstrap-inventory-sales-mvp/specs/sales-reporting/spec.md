# sales-reporting Specification Delta

## ADDED Requirements

### Requirement: Sales Invoice

The system SHALL create sales invoices and reduce inventory.

#### Scenario: Prevent overselling

- **GIVEN** a product has insufficient stock
- **WHEN** staff tries to sell more than available quantity
- **THEN** the system rejects the invoice
- **AND** shows a clear message

### Requirement: Revenue Report

The system SHALL report revenue by date range.

#### Scenario: View revenue

- **GIVEN** manager chooses a valid date range
- **WHEN** the report is loaded
- **THEN** revenue, invoice count and estimated profit are displayed

### Requirement: Top Customer And Product

The system SHALL show top selling products and top customers.

#### Scenario: View top products

- **WHEN** manager requests top selling products
- **THEN** the system returns products ordered by sold quantity or revenue
