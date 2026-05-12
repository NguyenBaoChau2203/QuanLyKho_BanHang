# dashboard-assistant Specification Delta

## ADDED Requirements

### Requirement: Management Dashboard

The system SHALL provide a dashboard for quick business overview.

#### Scenario: Open dashboard

- **WHEN** manager opens the main screen
- **THEN** the system displays today revenue, month revenue, invoice count and low stock count

### Requirement: Rule-Based Assistant

The system SHALL provide a rule-based assistant for common management questions.

#### Scenario: Ask low stock

- **WHEN** manager asks "hàng sắp hết"
- **THEN** the assistant returns low stock products

#### Scenario: Ask revenue

- **WHEN** manager asks "doanh thu hôm nay"
- **THEN** the assistant returns today's revenue summary
