## 1. Goal

Create a professional WinForms shell that is safe for future feature screens and supports demo navigation without backend dependence.

## 2. Scope

This phase may update only the shell and placeholder navigation experience:

- `FrmLogin`
- `FrmMain`
- sidebar navigation
- top bar
- content host
- status area
- placeholder screens where needed

It does not implement real master data, inventory, sales, reports, or assistant workflows.

## 3. UI design rules

- Use standard WinForms .NET 8 controls only.
- Prefer `Dock`, `TableLayoutPanel`, and `FlowLayoutPanel` for responsive layout.
- Use professional Vietnamese labels.
- Keep navigation stable and crash-free.
- Do not call DAL or write SQL from WinForms.

## 4. Validation

- `openspec validate phase-2-winforms-ui-shell`
- `dotnet build QuanLyKhoBanHang.sln`
- `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`

## 5. Risks

- Navigation handlers may break if existing forms are inconsistent.
  - Mitigation: load placeholders or existing forms safely.
- The shell may become too tightly coupled to backend services.
  - Mitigation: keep the shell independent and deterministic.
