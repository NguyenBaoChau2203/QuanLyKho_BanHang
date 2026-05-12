# inventory-stock Specification Delta

## ADDED Requirements

### Requirement: Product Catalog

The system SHALL allow warehouse staff to manage products, categories and suppliers.

#### Scenario: Product has stock settings

- **GIVEN** a product is created
- **WHEN** the product is saved
- **THEN** it includes cost price, selling price, current quantity and minimum stock level

### Requirement: Purchase Receipt

The system SHALL increase product stock when a purchase receipt is created.

#### Scenario: Import stock

- **GIVEN** a purchase receipt has valid product lines
- **WHEN** the receipt is saved
- **THEN** product quantities increase
- **AND** stock transactions are recorded

### Requirement: Stocktake

The system SHALL support stocktaking and stock adjustment.

#### Scenario: Stocktake difference

- **GIVEN** actual quantity differs from system quantity
- **WHEN** a stocktake is saved
- **THEN** the system records the difference
- **AND** updates current stock through a transaction
