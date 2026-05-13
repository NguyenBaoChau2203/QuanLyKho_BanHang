## Why

Phase 0 is needed to lock shared contracts before Châu, Dũ, and Hùng work in parallel. Without a stable database schema, DTO contract, and BLL service surface, the team will keep blocking each other and risk merge conflicts or broken builds.

## What Changes

- Finalize `database/schema.sql` as the shared database contract for inventory, purchasing, sales, customers, stock transactions, stocktakes, and reporting.
- Finalize `database/seed.sql` so the whole team has stable demo data for login and basic workflows.
- Finalize DTOs in `src/QuanLyKhoBanHang.DTO/` as the UI ↔ BLL contract.
- Finalize public BLL service method signatures in `src/QuanLyKhoBanHang.BLL/Services/` and standardize on `ServiceResult<T>` where appropriate.
- Explicitly exclude invoice printing from Phase 0 so no print contract is finalized yet.
- Define mock/stub behavior so Châu can build WinForms UI before backend implementation is finished.
- Define file ownership and contract change rules for Châu, Dũ, and Hùng.
- **BREAKING**: Treat any later change to column names, DTO property names, or public service signatures as a contract change that requires review and coordinated updates.

## Capabilities

### New Capabilities
- `contract-foundation`: Stable schema, DTO, service, mock/stub, and ownership rules that let the team work in parallel without waiting on each other.

### Modified Capabilities
- `bootstrap-inventory-sales-mvp`: Narrow the MVP work so it depends on the Phase 0 contract instead of requiring implementation details to be decided later.

## Impact

- `database/schema.sql` and `database/seed.sql` become the authoritative shared contract for LocalDB.
- DTOs in `src/QuanLyKhoBanHang.DTO/` must stay stable enough for WinForms to consume directly.
- Public services in `src/QuanLyKhoBanHang.BLL/Services/` must keep names, parameters, and return types stable for all three branches.
- WinForms can be developed against stubbed service behavior while DAL/BLL implementation is still pending.
- OpenSpec change `bootstrap-inventory-sales-mvp` must align with the Phase 0 contract boundaries.
