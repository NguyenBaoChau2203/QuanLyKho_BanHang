# Antigravity OpenSpec Workflow

Tài liệu này dùng khi làm việc với Google Antigravity trong project `QuanLyKho_BanHang`.

## File Antigravity Đọc

Project có hai file rule chính:

```text
AGENTS.md
GEMINI.md
```

- `AGENTS.md`: luật chung cho nhiều agent/tool.
- `GEMINI.md`: luật riêng cho Antigravity/Gemini, có thể override hoặc bổ sung `AGENTS.md`.

Khi mở project trong Antigravity, hãy mở đúng workspace:

```text
D:\QuanLyKho_BanHang
```

## Nguyên Tắc OpenSpec

Không code tính năng lớn trước khi có OpenSpec change.

Luồng chuẩn:

1. Tạo proposal.
2. Validate OpenSpec.
3. Review/approve.
4. Implement.
5. Validate/build/test.
6. Tick `tasks.md`.
7. Commit nếu được yêu cầu.

## Lệnh Validate

Nếu đã cài OpenSpec global:

```powershell
openspec validate <change-name>
```

Nếu chưa cài global:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

## Prompt Tạo Proposal Trong Antigravity

```text
Follow AGENTS.md and GEMINI.md.

Create an OpenSpec proposal for change:
<change-name>

Before writing files:
- Run git status -sb.
- Read AGENTS.md, GEMINI.md, README.md, docs/05_OpenSpecWorkflow.md, docs/07_ContractFoundation.md, and related existing OpenSpec changes.

Create:
- openspec/changes/<change-name>/proposal.md
- openspec/changes/<change-name>/design.md
- openspec/changes/<change-name>/tasks.md
- openspec/changes/<change-name>/specs/<capability>/spec.md

Do not implement application code yet.
Validate the OpenSpec change after creating artifacts.
```

## Prompt Implement Trong Antigravity

```text
Follow AGENTS.md and GEMINI.md.

Implement approved OpenSpec change:
<change-name>

Before editing:
- Run git status -sb.
- Read proposal.md, design.md, tasks.md, and spec.md for this change.

Rules:
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
```

## Prompt Review Trong Antigravity

```text
Follow AGENTS.md and GEMINI.md.

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

## Lưu Ý Riêng Cho Project Này

- Châu phụ trách UI, Admin, Manager role, OpenSpec, database tổng thể, tích hợp.
- Dũ phụ trách backend kho.
- Hùng phụ trách backend bán hàng/báo cáo.
- Những tính năng Admin như quản lý tài khoản, phân quyền, audit log thuộc phạm vi Châu nếu chưa có phân công mới.
- Nếu Antigravity agent muốn sửa nhiều mảng cùng lúc, yêu cầu tách thành OpenSpec change nhỏ.

## Khi Agent Báo Không Biết OpenSpec

Hãy dán câu mở đầu này:

```text
This repository uses OpenSpec/SDD. Follow AGENTS.md, GEMINI.md, and docs/10_AntigravityOpenSpecWorkflow.md. Create or implement OpenSpec changes before touching application code.
```
