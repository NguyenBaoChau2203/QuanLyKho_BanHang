## Why

The current manager assistant is deterministic and useful for demos, but it only recognizes a small set of rule-based Vietnamese command phrases. Phase 10 upgrades it into a hybrid assistant that can optionally use DeepSeek for better natural-language understanding and friendlier final wording while keeping the app safe to demo without any API key.

## What Changes

- Add an optional DeepSeek provider inside the BLL assistant flow.
- Keep the existing rule-based assistant as the guaranteed offline fallback.
- Read DeepSeek configuration only from environment variables:
  - `DEEPSEEK_API_KEY`
  - optional `DEEPSEEK_MODEL`
  - optional `DEEPSEEK_BASE_URL`
- Default to model `deepseek-chat` and base URL `https://api.deepseek.com`.
- Extend `AssistantResponseDto` with mode/status metadata so WinForms can show:
  - AI online
  - Offline rule-based
  - AI failed, fallback used
- Keep `AssistantService.Ask(string question)` as the public WinForms entry point returning `ServiceResult<AssistantResponseDto>`.
- Add a small `AssistantService.GetModeStatus()` helper so WinForms can render initial mode/status without reading environment variables directly.
- Update the assistant UI to show the current mode clearly and make the assistant easier to find from the main sidebar.
- Add focused tests for missing API key, AI failure fallback, existing demo commands, and non-throwing behavior for empty/unknown/failure cases.

## Architecture Constraints

- Preserve strict WinForms -> BLL -> DAL -> DTO.
- WinForms must call `AssistantService` only for assistant behavior.
- WinForms must not call DeepSeek directly.
- WinForms must not call DAL directly.
- WinForms must not contain SQL.
- DeepSeek provider must live inside BLL assistant flow.
- DeepSeek must not access DAL directly.
- DeepSeek must not generate SQL to execute.
- Real business data must still come from existing BLL services.
- No API key may be stored in source code or committed to GitHub.

## Functional Scope

- Existing five commands must still work without an API key:
  - `doanh thu hôm nay`
  - `hàng sắp hết`
  - `top sản phẩm bán chạy`
  - `khách hàng mua nhiều nhất`
  - `kiểm kê hôm nay`
- When `DEEPSEEK_API_KEY` is available, DeepSeek may only:
  - classify Vietnamese natural-language questions into approved assistant intents
  - generate a friendly final answer from safe BLL-produced data
- If the key is missing, invalid, network fails, timeout occurs, quota fails, or the response is invalid, the assistant must not crash and must return deterministic rule-based fallback.

## Non-Goals

- Do not train any ML model.
- Do not implement vector database.
- Do not implement full RAG document retrieval.
- Do not allow AI-generated SQL execution.
- Do not replace the existing assistant completely.
- Do not add third-party UI frameworks.
- Do not implement unrelated screens.

## Impact

- BLL gains small assistant provider/helper classes for rule-based and DeepSeek orchestration.
- DTO gains backward-compatible status properties.
- WinForms assistant screen can display service-owned mode/status while staying on BLL contracts.
- Demo remains fully usable without network access or DeepSeek configuration.
