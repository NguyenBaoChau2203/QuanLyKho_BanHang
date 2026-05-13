# Codex OpenSpec Workflow

Tài liệu này thay thế workflow Cursor `/opsx:*` khi làm việc trong Codex app.

## Skill Đã Thiết Lập

Codex skill đã được tạo ở hai nơi:

```text
C:\Users\chau1\.codex\skills\openspec-sdd
D:\QuanLyKho_BanHang\.agent\skills\openspec-sdd
```

Khi muốn Codex làm việc theo OpenSpec, bắt đầu prompt bằng:

```text
Use the openspec-sdd skill.
```

Codex app không cần dùng `/opsx:propose` hoặc `/opsx:apply` như Cursor. Thay vào đó, bạn mô tả thao tác bằng prompt rõ ràng.

Nếu Codex báo `openspec-sdd` chưa có trong danh sách skill của phiên, vẫn có thể tiếp tục theo workflow thủ công vì toàn bộ hướng dẫn đã nằm trong repo. Khi đó dùng câu mở đầu:

```text
Follow the repository OpenSpec/SDD workflow from AGENTS.md and docs/09_CodexOpenSpecWorkflow.md.
```

Sau khi restart Codex app hoặc mở phiên mới, repo-local skill ở `.agent/skills/openspec-sdd` sẽ giúp agent nhận workflow dễ hơn.

## Validate OpenSpec

Nếu máy đã cài OpenSpec global:

```powershell
openspec validate <change-name>
```

Nếu chưa cài global, dùng lệnh an toàn:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

Ví dụ:

```powershell
npx --yes --package @fission-ai/openspec openspec validate phase-10-hybrid-ai-assistant-deepseek
```

## Prompt Tạo Proposal

Dùng khi muốn tạo OpenSpec change mới nhưng chưa code:

```text
Use the openspec-sdd skill.

Create an OpenSpec proposal for change:
<change-name>

Before writing files:
- Run git status -sb.
- Read AGENTS.md, README.md, docs/05_OpenSpecWorkflow.md, docs/07_ContractFoundation.md, and related existing OpenSpec changes.

Create:
- openspec/changes/<change-name>/proposal.md
- openspec/changes/<change-name>/design.md
- openspec/changes/<change-name>/tasks.md
- openspec/changes/<change-name>/specs/<capability>/spec.md

Do not implement application code yet.
Validate the change after creating artifacts.
```

## Prompt Implement Change

Dùng khi proposal đã ổn và bạn muốn Codex thực hiện:

```text
Use the openspec-sdd skill.

Implement OpenSpec change:
<change-name>

Rules:
- Run git status -sb before editing.
- Read proposal.md, design.md, tasks.md, and spec.md for this change.
- Keep implementation inside approved scope.
- Preserve WinForms -> BLL -> DAL -> DTO.
- WinForms must not call DAL directly.
- WinForms must not contain SQL.
- Do not change DTO/service contracts unless the OpenSpec change explicitly requires it.
- Mark tasks complete only after validation/build/test pass.

Run:
- npx --yes --package @fission-ai/openspec openspec validate <change-name>
- dotnet build QuanLyKhoBanHang.sln
- dotnet test QuanLyKhoBanHang.sln --no-build --no-restore

Commit with:
<commit-message>
```

## Prompt Review Change

Dùng khi muốn Codex review không sửa file:

```text
Use the openspec-sdd skill.

Review OpenSpec change:
<change-name>

Do not modify files.
Check:
- OpenSpec validity
- architecture compliance
- scope drift
- build/test risk
- missing tasks
- demo risk

Output findings first, ordered by severity.
```

## Prompt Cho Phase 10 DeepSeek

```text
Use the openspec-sdd skill.

Create an OpenSpec proposal for change:
phase-10-hybrid-ai-assistant-deepseek

Goal:
Upgrade the current rule-based manager assistant into a hybrid assistant:
- AI online mode using DeepSeek API when DEEPSEEK_API_KEY is available.
- Offline fallback mode using the existing rule-based assistant when API is unavailable.
- Keep the app demo-safe and do not make DeepSeek required.

Scope:
- WinForms still calls AssistantService only.
- AssistantService may orchestrate rule-based and DeepSeek providers.
- DeepSeek must not access DAL directly.
- DeepSeek must not generate SQL to execute.
- DeepSeek only helps understand Vietnamese natural-language questions and generate friendly final answers from safe BLL data.
- Real business data must still come from BLL services.
- If API key is missing, network fails, quota fails, or DeepSeek returns invalid output, AssistantService must fall back to current rule-based behavior.

Architecture rules:
- Preserve WinForms -> BLL -> DAL -> DTO.
- No API key in source code.
- No API key committed to GitHub.
- Read key from environment variable DEEPSEEK_API_KEY.
- Optional environment variables: DEEPSEEK_MODEL and DEEPSEEK_BASE_URL.
- Default model may be deepseek-chat.
- Default base URL may be https://api.deepseek.com

Acceptance criteria:
- Existing suggested commands still work without API key:
  - doanh thu hôm nay
  - hàng sắp hết
  - top sản phẩm bán chạy
  - khách hàng mua nhiều nhất
  - kiểm kê hôm nay
- With no DEEPSEEK_API_KEY, app builds, runs, and answers using rule-based fallback.
- With invalid API key or API failure, app must not crash.
- UI shows assistant mode clearly: AI online, offline rule-based, or AI failed fallback.
- Tests cover fallback behavior.

Out of scope:
- Do not train an ML model.
- Do not implement vector database.
- Do not implement full RAG document retrieval in this phase.
- Do not allow AI-generated SQL execution.
- Do not replace the existing assistant completely.

Do not implement application code yet.
Validate the OpenSpec change after creating artifacts.
```
