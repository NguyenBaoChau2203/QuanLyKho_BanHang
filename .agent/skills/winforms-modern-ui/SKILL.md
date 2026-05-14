---
name: winforms-modern-ui
description: Use when building, restyling, or polishing QuanLyKhoBanHang WinForms screens, sidebar, dashboard, CRUD forms, inventory/sales workflows, reports, assistant UI, or GPT Image 2 mockup handoff into reusable C# WinForms components.
---

# WinForms Modern UI

## First Steps

- Read `AGENTS.md`, `docs/04_QuyChuanChung.md`, and `docs/12_WinFormsModernUiWorkflow.md`.
- If the task is a large screen/phase, read the related OpenSpec change in `openspec/changes/`.
- Preserve the architecture: WinForms -> BLL -> DAL -> DTO. Do not call DAL or write SQL in WinForms.

## Implementation Rules

- Sidebar belongs only in `Forms/Main/FrmMain.cs`. Do not create per-screen sidebars.
- Use the shared UI foundation in `Forms/Common`: `AppTheme`, `UiFactory`, `RoundedPanel`.
- Use `FontAwesome.Sharp` icons. Do not request or create PNG icons unless the user explicitly wants image assets.
- Prefer `TableLayoutPanel`, `FlowLayoutPanel`, `Dock`, `Anchor`, and fixed heights for stable WinForms layouts.
- Style grids with `UiFactory.StyleGrid`.
- Keep visible text Vietnamese; keep code identifiers English.
- Keep UI helpers presentation-only and backend-independent.
- Do not repeat the active screen name inside the first content header when `FrmMain` already shows it in the shell header. Use `UiFactory.SectionHeader(...)` in the Dashboard style with a specific section title, subtitle, and FontAwesome icon; keep right-side summary/count labels vertically centered, primary-colored, and non-wrapping.

## Mockup Guidance

- Do not ask for GPT Image 2 mockups for simple CRUD/list screens.
- Ask for mockups for complex workflows: sales invoice, purchase receipt, stocktake, report/dashboard variants, assistant/chat.
- GPT Image 2 prompts must say: no sidebar, no full app shell, WinForms-friendly controls, light desktop business software style, and background-only export if a background is used.

## Validation

Run:

```powershell
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

If the WinForms `.exe` is locked, close the running app and build again.

## Visual QA Modes

- Default token-saving mode: do not open the app automatically. Run build/test, inspect code/layout carefully, and ask the user for a screenshot if a visual issue is suspected.
- Full app-window QA mode: only open the actual WinForms app or edited form when the user explicitly asks for it.
- Use the demo login for app review: username `admin`, password `admin123`.
- If login has remembered credentials, click the login button directly.
- When doing full app-window QA, inspect sidebar, header, content area, cards, icons, buttons, and grids.
- Fix obvious visual defects: clipped icons, clipped Vietnamese text, overlaps, cramped rows, broken spacing, or controls hidden by docking.
- Mention in the final response which QA mode was used.
