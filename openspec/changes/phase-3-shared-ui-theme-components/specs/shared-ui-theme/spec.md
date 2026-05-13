# Shared UI Theme and Components

## ADDED Requirements

### Requirement: Shared UI theme helpers exist
The system MUST provide shared WinForms theme and helper infrastructure so screens can reuse consistent styling without duplicating layout code.

#### Scenario: Forms use shared helpers
- **WHEN** a new screen or placeholder needs cards, panels, or standard spacing
- **THEN** it can use shared theme/helper APIs instead of hard-coded styling in every form

### Requirement: Shared UI helpers stay backend independent
The system MUST keep shared UI helpers free from DAL access, SQL, and business logic.

#### Scenario: Helpers remain presentation-only
- **WHEN** the helper layer is used by the shell or placeholder screens
- **THEN** it only creates or styles controls
- **AND** it does not call services that access DAL or SQL directly
