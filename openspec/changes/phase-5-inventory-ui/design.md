## Design

- Each screen uses the common `CrudListForm`/shared WinForms styling patterns.
- `FrmPurchaseReceipt` focuses on fast line entry and visible receipt totals.
- `FrmInventory` shows current stock and low-stock alerts using a dual-grid layout.
- `FrmStocktake` shows system quantity, actual quantity, and difference columns for comparison.
- All data interactions remain on top of BLL service contracts and deterministic stub data until backend implementations are ready.
