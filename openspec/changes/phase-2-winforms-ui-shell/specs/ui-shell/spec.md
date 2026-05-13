# WinForms UI Shell

## ADDED Requirements

### Requirement: Application shell provides stable navigation
The system MUST provide a polished WinForms shell with login, main window, sidebar navigation, top bar, content host, and status area that can open existing screens or placeholders safely.

#### Scenario: User enters the app shell
- **WHEN** the user signs in through `FrmLogin`
- **THEN** the application opens `FrmMain` with a responsive shell layout
- **AND** the shell shows Vietnamese labels and a clear navigation structure

#### Scenario: Navigation loads safely
- **WHEN** the user clicks any main menu item
- **THEN** the shell loads an existing screen or a placeholder form without crashing
- **AND** the content area updates inside the main host panel

### Requirement: Shell remains backend independent
The system MUST keep the WinForms shell independent from DAL and SQL access.

#### Scenario: UI does not depend on real backend implementation
- **WHEN** the shell renders forms or placeholder content
- **THEN** it does not call DAL directly
- **AND** it does not embed SQL in WinForms code
