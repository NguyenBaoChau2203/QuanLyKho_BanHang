## Design

### Screen composition

Each master data form uses the same pattern:

- top search/filter strip,
- main `DataGridView` bound through `BindingSource`,
- right-side or bottom edit panel with input fields,
- action row with Add, Edit, Save, Cancel, Deactivate, Refresh,
- empty/error state panel when no rows or when service returns a failure.

### Data strategy

Forms first call the appropriate BLL service contract. When the service returns empty data or a failure message, the form falls back to deterministic in-memory stub rows so the UI remains demoable.

### Contract safety

- No DTO contract changes are required for this phase.
- No DAL or SQL usage is allowed in WinForms.
- All forms use `ServiceResult<T>` and existing DTOs only.

### Navigation

`FrmMain` exposes direct navigation buttons for all four master data forms. Each screen loads into the existing shell host without breaking layout or navigation state.