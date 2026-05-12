## Why

Châu needs a polished sales invoice screen so product search, customer selection, line editing, and total calculation can be demoed independently of backend readiness.

## What Changes

- Implement `FrmSalesInvoice`.
- Add product search/select flows, customer selection, line item grid, and totals/discount summary.
- Use shared WinForms UI helpers from phase 3 for consistent styling.
- Support deterministic stub/mock data through existing BLL service contracts when backend data is not ready.
- Keep all UI code in WinForms and avoid DAL/SQL access entirely.

## Impact

- Sales invoice workflow becomes demoable in isolation.
- The shell can open the sales screen from `FrmMain`.
- Future backend integration can be swapped in without redesigning the screen.
