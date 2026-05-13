## 1. Goal

Provide shared UI infrastructure so WinForms screens can reuse theme, spacing, and component styling consistently.

## 2. Scope

This phase may add only shared UI helpers and apply them lightly to the shell:

- `AppTheme`
- `UiFactory`
- DataGridView styling helper
- KPI card factory or control
- toolbar/search panel helper
- empty/error state helper

It may also refactor the shell or placeholder screens minimally to use the shared helpers.

It does not implement business screens, DAL access, or SQL.

## 3. UI rules

- Use standard WinForms .NET 8 controls only.
- Keep helpers simple, deterministic, and reusable.
- Prefer Vietnamese labels in visible UI strings.
- Keep the helpers free of data access and service logic.

## 4. Validation

- `openspec validate phase-3-shared-ui-theme-components`
- `dotnet build QuanLyKhoBanHang.sln`
- `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`

## 5. Risks

- Shared helpers could become too opinionated or hard to reuse.
  - Mitigation: keep APIs small and form-friendly.
- Styling changes may accidentally affect navigation flow.
  - Mitigation: only apply helpers where safe in shell/placeholder screens.
