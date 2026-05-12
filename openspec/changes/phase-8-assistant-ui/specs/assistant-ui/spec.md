# Manager Assistant UI

## ADDED Requirements

### Requirement: Assistant screen behaves like a deterministic manager command panel
The system MUST provide a WinForms assistant screen with Vietnamese labels that includes a command input, primary send action, suggested command buttons, a scrollable conversation history, readable assistant response blocks, and a clear/reset conversation action.

#### Scenario: User runs a suggested command
- **WHEN** the user clicks a suggested command such as `doanh thu hôm nay`
- **THEN** the assistant submits the command and appends both the user prompt and an assistant response card
- **AND** no external AI or network API is called

#### Scenario: Assistant uses BLL contracts first
- **WHEN** the user submits any command
- **THEN** the screen calls `AssistantService.Ask` before rendering a final answer
- **AND** recognized operational intents trigger additional reads through `ReportService`, `InventoryService`, or `StocktakeService` as appropriate

#### Scenario: Service data is empty
- **WHEN** a backing service returns an empty list or an informational empty message
- **THEN** the assistant substitutes deterministic stub content so the demo remains useful
- **AND** the UI clearly indicates demo fallback content

#### Scenario: Unknown command
- **WHEN** the assistant cannot map the input to a known intent
- **THEN** the UI shows a polite fallback explanation from `AssistantService` without crashing

#### Scenario: Presentation-only WinForms layer
- **WHEN** the assistant loads or refreshes answers
- **THEN** WinForms code does not reference DAL types and does not embed SQL strings
