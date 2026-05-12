## Why

Phase 9 is the final demo-readiness pass. The WinForms app already has the main screens, but the experience still needs polish: spacing, labels, grid styling, empty/error handling, resize behavior, and careful integration to any backend services that already exist and match the Phase 0 contract.

## What Changes

- Polish the existing WinForms screens for demo readiness:
  - Login
  - Main shell
  - Dashboard
  - Master data
  - Customers
  - Purchase receipt
  - Inventory
  - Stocktake
  - Sales invoice
  - Reports
  - Assistant
- Improve layout details:
  - spacing and paddings
  - label clarity
  - grid style consistency
  - button sizes
  - resize behavior
  - keyboard/focus flow where simple
  - empty and error states
- Connect WinForms to real BLL services only when the implementation already exists and matches the Phase 0 contract.
- Keep deterministic fallback data for unfinished backend areas.
- Update the final demo checklist with the recommended demo scenario.

## Non-Goals

- Do not add repositories or new DAL/database work.
- Do not introduce third-party UI frameworks.
- Do not add large new features.
- Do not change public contracts except tiny bug fixes that are documented.

## Impact

- The application becomes safer to demo and more consistent across screens.
- Backend integration remains contract-safe and limited to services that already exist.