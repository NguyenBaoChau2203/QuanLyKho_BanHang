## 1. OpenSpec

- [x] 1.1 Create proposal, design, tasks, and spec delta for `phase-10-hybrid-ai-assistant-deepseek`.
- [x] 1.2 Validate the OpenSpec change before implementation.

## 2. BLL assistant flow

- [x] 2.1 Keep `AssistantService.Ask(string question)` returning `ServiceResult<AssistantResponseDto>`.
- [x] 2.2 Move deterministic rule-based assistant behavior into a provider/helper inside BLL.
- [x] 2.3 Add optional DeepSeek provider/client using `HttpClient` and `System.Text.Json`.
- [x] 2.4 Read DeepSeek settings from environment variables only.
- [x] 2.5 Catch missing key, invalid key, network, timeout, quota, and invalid-response failures and fall back to rule-based behavior.
- [x] 2.6 Add a BLL-owned mode/status helper for the assistant UI.

## 3. DTO and UI

- [x] 3.1 Extend `AssistantResponseDto` with mode/status metadata.
- [x] 3.2 Update `FrmAssistant` to show AI online, offline rule-based, or AI failed fallback status from the service.
- [x] 3.3 Add a clear assistant entry to the main sidebar and clarify the top quick action label.

## 4. Tests

- [x] 4.1 Cover missing `DEEPSEEK_API_KEY` rule-based fallback.
- [x] 4.2 Cover AI provider failure fallback.
- [x] 4.3 Cover all five existing assistant commands.
- [x] 4.4 Cover empty, unknown, and API failure cases without thrown exceptions.

## 5. Validation

- [x] 5.1 Re-run OpenSpec validation.
- [x] 5.2 Run `dotnet build QuanLyKhoBanHang.sln`.
- [x] 5.3 Run `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.
- [x] 5.4 Search WinForms for DAL references.
- [x] 5.5 Search WinForms for SQL strings.
- [x] 5.6 Confirm WinForms project does not reference DAL.
- [x] 5.7 Confirm no API key appears in tracked files.

## 6. Finish

- [x] 6.1 Mark tasks complete only after validation/build/test/architecture checks pass.
- [x] 6.2 Commit with `feat(ai): add optional deepseek assistant fallback`.
