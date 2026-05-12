## Why

Managers need a credible “command panel” assistant for demos without wiring a real AI API. The assistant must feel responsive, readable, and aligned with reporting/inventory questions while staying strictly on BLL contracts.

## What Changes

- Implement `FrmAssistant` with command input, scrollable conversation history, suggested command chips, and assistant response cards.
- Route recognized Vietnamese commands to `AssistantService.Ask` first, then enrich answers using `ReportService`, `InventoryService`, and `StocktakeService` when the intent matches operational questions.
- Use deterministic stub content whenever BLL returns empty lists so the demo stays meaningful.
- Provide clear fallback copy when the assistant cannot classify a question.
- Add reset/clear conversation affordance.
- Remain offline: no AI endpoints, no network calls, no DAL usage from WinForms, no SQL in WinForms.

## Impact

- Demo storyline gains a polished assistant surface before Phase 9 backend integration.
- Stub behavior stays predictable and replaces empty service results transparently.
