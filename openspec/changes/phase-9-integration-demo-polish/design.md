## Design Overview

Phase 9 is a UI finishing pass. The implementation should keep the existing app structure and improve presentation consistency rather than redesigning workflows.

## Principles

- Stay within the current three-layer contract: WinForms -> BLL -> DAL -> DTO.
- WinForms talks only to BLL services.
- Prefer deterministic stub data over inventing missing backend behavior.
- Only use real service calls when the service already exists and returns data compatible with the Phase 0 contract.
- Keep screen behavior predictable for a live demo.

## Screen Review Targets

### Login

- Keep the form simple and stable.
- Improve default focus and Enter-key flow.
- Preserve direct login feedback through `AuthService`.

### Main Shell

- Keep the navigation shell consistent.
- Tighten spacing in sidebar, header, and content area.
- Ensure content switching remains smooth and the status text is useful.

### Dashboard

- Preserve KPI cards and list sections.
- Show empty-state fallback data when services return nothing.
- Format money and dates consistently.
- Keep the dashboard scannable in a few seconds.

### Master Data, Customers, Purchase Receipt, Inventory, Stocktake, Sales Invoice, Reports, Assistant

- Standardize labels, grid styling, and button sizing.
- Use clear empty/error messages.
- Improve resize behavior with `Dock`, `Anchor`, and layout panels.
- Keep editors usable for fast demo interaction.
- Reuse fallback data when backend data is missing or service calls are not ready.

## Integration Rules

- If a BLL service implementation already exists and matches the contract, call it directly from WinForms.
- If a service is only partially implemented, keep the screen deterministic with fallback data instead of writing DAL calls or duplicating business logic.
- Do not add DAL references to WinForms.
- Do not place SQL in WinForms.

## Validation Plan

- Verify WinForms has no DAL references.
- Verify no SQL keywords appear in WinForms.
- Verify the WinForms project does not reference DAL.
- Build the solution.
- Run tests without rebuilding.
- Re-run OpenSpec validation after the UI polish is complete.