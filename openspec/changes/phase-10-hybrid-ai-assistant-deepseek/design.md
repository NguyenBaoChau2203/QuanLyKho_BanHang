## Design Overview

Phase 10 keeps the assistant synchronous and demo-safe. `AssistantService` remains the only public UI entry point and orchestrates two providers:

- Rule-based provider: deterministic, always available, owns the current command matching and BLL data shaping.
- DeepSeek provider: optional, enabled only when `DEEPSEEK_API_KEY` is present, and allowed to work only from safe BLL-produced context.

## Assistant Flow

1. `FrmAssistant` sends the user question to `AssistantService.Ask`.
2. `AssistantService` validates the question.
3. The rule-based provider prepares deterministic fallback data and safe answer context from existing BLL services.
4. If `DEEPSEEK_API_KEY` is missing, `AssistantService` returns the rule-based response with offline mode metadata.
5. If `DEEPSEEK_API_KEY` is present, `AssistantService` calls the DeepSeek provider with:
   - the user question
   - the allowed intent names
   - safe BLL answer summaries
   - instructions to return JSON only
6. If DeepSeek returns a valid approved intent and answer, the service returns AI online mode.
7. If DeepSeek throws, times out, returns non-success HTTP, returns invalid JSON, or returns unsupported content, the service returns the deterministic rule-based response with failed-fallback mode metadata.

## DeepSeek Configuration

- `DEEPSEEK_API_KEY` enables the online provider.
- `DEEPSEEK_MODEL` overrides the default `deepseek-chat`.
- `DEEPSEEK_BASE_URL` overrides the default `https://api.deepseek.com`.
- The provider calls `POST /chat/completions`.
- Timeout is short so the WinForms demo does not hang.
- API exceptions are caught inside BLL and never thrown to WinForms.

## Data Safety

DeepSeek receives only safe text summaries produced by existing BLL services. It does not receive database access, DAL types, connection strings, or SQL. It may choose among known intents and rewrite a final answer, but it cannot ask the app to execute generated SQL.

## DTO Contract

`AssistantResponseDto` will be extended with backward-compatible metadata:

- `Mode`: machine-readable mode string such as `ai-online`, `offline-rule-based`, or `ai-failed-fallback`
- `StatusMessage`: user-facing Vietnamese status text suitable for the assistant UI
- `IsFallback`: whether deterministic fallback was used

Existing `Intent`, `Answer`, `Handled`, and `CreatedAt` remain.

## WinForms UI

The assistant screen should render only the answer and status returned by `AssistantService`. It may call `AssistantService.GetModeStatus()` on load to display the initial mode label, and `AssistantService.Ask(...)` for user questions. It should no longer duplicate report/inventory/stocktake data shaping outside the assistant service. The sidebar label should make the feature discoverable as `Trợ lý AI`, and the quick action label should be clearer than the previous generic `Trợ lý`.

## Tests

Tests should construct `AssistantService` in deterministic modes:

- No API key returns offline rule-based mode and useful answers.
- A failing HTTP provider returns AI failed fallback mode and useful answers.
- The existing five demo commands still succeed.
- Empty, unknown, and API-failure inputs do not throw.
