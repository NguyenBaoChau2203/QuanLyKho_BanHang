## Why

Châu needs polished master data screens so product, category, supplier, and customer operations can be demoed independently of backend readiness.

## What Changes

- Implement `FrmProduct`, `FrmCategory`, `FrmSupplier`, and `FrmCustomer`.
- Add search/filter rows, grid-based browsing, edit panels, and standard action buttons.
- Use shared WinForms UI helpers from phase 3 for consistent styling.
- Support deterministic stub/mock data through existing BLL service contracts when backend data is not ready.
- Keep all UI code in WinForms and avoid DAL/SQL access entirely.

## Impact

- Master data workflows become usable and reviewable in isolation.
- The shell can open all master data screens from `FrmMain`.
- Future backend integration can be swapped in without redesigning the screens.