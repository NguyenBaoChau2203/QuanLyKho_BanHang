## Why

Châu needs dedicated inventory workflow screens so nhập kho, tồn kho và kiểm kê can be demoed independently of backend readiness.

## What Changes

- Implement `FrmPurchaseReceipt`, `FrmInventory`, and `FrmStocktake`.
- Add product search/select flows, line item grids, and summary/validation panels.
- Use shared WinForms UI helpers from phase 3 for consistent styling.
- Support deterministic stub/mock data through existing BLL service contracts when backend data is not ready.
- Keep all UI code in WinForms and avoid DAL/SQL access entirely.
