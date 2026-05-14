# GEMINI.md

Antigravity-specific instructions for this repository.

## Project Context

This is a Vietnamese `.NET 8 WinForms` final project:

```text
Quản lý kho & bán hàng
```

The project uses a strict 3-layer architecture:

```text
WinForms -> BLL -> DAL -> DTO
```

Antigravity agents must preserve this architecture.

## Required First Step

Before editing files, always run:

```powershell
git status -sb
```

If the user names an expected branch, also run:

```powershell
git branch --show-current
```

If the working tree is dirty, report the dirty files before starting unless the user explicitly asks to continue the current dirty work.

## OpenSpec / SDD Workflow

This project uses OpenSpec. For large features, do not code first.

Use this order:

1. Create OpenSpec proposal artifacts.
2. Validate the OpenSpec change.
3. Wait for user approval if the user asks for proposal-only work.
4. Implement only the approved scope.
5. Run validation/build/test.
6. Mark tasks complete.
7. Commit only when the user asks or the prompt explicitly requests a commit.

OpenSpec change structure:

```text
openspec/changes/<change-name>/
  proposal.md
  design.md
  tasks.md
  specs/<capability>/spec.md
```

Validate with:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

If global `openspec` is installed, this is also acceptable:

```powershell
openspec validate <change-name>
```

## Architecture Guardrails

- WinForms must not call DAL directly.
- WinForms must not contain SQL.
- DAL must not contain UI logic.
- DTO must remain data-only.
- BLL owns validation and orchestration.
- Do not add third-party UI frameworks.
- Do not hard-code API keys or secrets.
- DeepSeek/OpenAI/API keys must come from environment variables only.

## WinForms UI Rules

- Use standard WinForms controls.
- Keep operational app feel: clean, practical, readable.
- Vietnamese labels must not be clipped.
- Sidebar buttons must keep fixed height and must not stretch into empty space.
- Use existing `AppTheme` and `UiFactory` whenever possible.
- No landing-page style screens.
- No decorative UI that is difficult to implement in WinForms.

## Admin/Role Ownership

Châu owns:

- Admin UI
- account management UI
- role/permission UI
- audit log UI
- Manager role experience
- login demo flow
- sidebar/menu authorization
- final UI integration

Dũ owns inventory/backend warehouse services.

Hùng owns sales/report/backend services.

## Verification Commands

Run after implementation:

```powershell
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

Architecture checks:

```powershell
Get-ChildItem -Path src\QuanLyKhoBanHang.WinForms -Recurse -File -Filter *.cs | Select-String -Pattern 'QuanLyKhoBanHang.DAL|SqlConnection|SqlCommand|SELECT |INSERT |UPDATE |DELETE |FROM ' -CaseSensitive
Select-String -Path src\QuanLyKhoBanHang.WinForms\QuanLyKhoBanHang.WinForms.csproj -Pattern 'QuanLyKhoBanHang.DAL|PackageReference'
```

## Antigravity Prompt Pattern

For proposal-only work:

```text
Follow AGENTS.md and GEMINI.md.

Create an OpenSpec proposal for change:
<change-name>

Do not implement application code yet.
Validate the OpenSpec change.
```

For implementation work:

```text
Follow AGENTS.md and GEMINI.md.

Implement approved OpenSpec change:
<change-name>

Run OpenSpec validation, build, and tests.
Mark tasks complete only after everything passes.
```
