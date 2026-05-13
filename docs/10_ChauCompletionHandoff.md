# Chau Completion Handoff

## Branch

Current local branch:

```text
feature/project-lead-chau
```

This branch is broader than UI. It contains Chau's project-lead scope: OpenSpec, docs, database foundation, contracts, WinForms UI, integration shell, demo auth/admin foundation, assistant integration, validation, and handoff planning.

## Current Status

Chau's own planned foundation/demo work is complete enough to merge into `main` before Du and Hung continue backend implementation.

Completed on this branch:

- Project architecture docs and OpenSpec workflow.
- Database schema and seed foundation for demo.
- DTO and BLL service contract foundation.
- WinForms shell and navigation.
- Shared WinForms theme/helpers.
- Master data, inventory, sales, dashboard, report, and assistant UI.
- Optional DeepSeek assistant with offline rule-based fallback.
- Admin demo features:
  - role-based demo login
  - account management stub UI
  - role/permission matrix UI
  - audit log viewer UI
  - role-based sidebar and quick actions
- Phase 12 OpenSpec and assignment docs for real Auth/Admin DAL.

Validation already passed after the latest implementation:

```powershell
npx --yes --package @fission-ai/openspec openspec validate phase-11-admin-user-permission-audit
npx --yes --package @fission-ai/openspec openspec validate phase-12-auth-admin-real-dal-security
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

## What Remains For Chau

Before final submission, Chau only needs to:

- Merge completed backend branches from Du and Hung when they are ready.
- Review any schema, seed, DTO, or public service contract changes before merging.
- Re-run OpenSpec validation, build, test, and demo checklist after each backend merge.
- Do final UI polish if desired.

Chau should not need to implement the real inventory/sales/auth/audit DAL on this branch unless the team explicitly changes ownership.

## Work Assigned To Du

Du continues on `feature/inventory-du`.

Primary scope:

- Inventory backend:
  - categories
  - products
  - suppliers
  - purchase receipts
  - inventory
  - stocktakes
- Phase 12 identity backend:
  - `Users`
  - `Roles`
  - permissions if schema is approved
  - account CRUD DAL/BLL
  - password hashing helper if Du owns the account backend

Du should avoid editing WinForms files unless Chau asks for a UI integration fix.

## Work Assigned To Hung

Hung continues on `feature/sales-report-hung`.

Primary scope:

- Sales/report backend:
  - customers
  - sales invoices
  - sales invoice details
  - revenue reports
  - top products/customers
  - assistant commands related to sales/reporting
- Phase 12 audit backend:
  - `AuditLogs` repository
  - audit filtering
  - audit writer methods
  - auth/admin audit tests

Hung should avoid editing WinForms files unless Chau asks for a UI integration fix.

## Conflict Guidance

Expected low-conflict areas:

- Chau owns `src/QuanLyKhoBanHang.WinForms/`, `docs/`, `openspec/`, and final database script review.
- Du owns inventory-related DAL/BLL implementation and tests.
- Hung owns sales/report-related DAL/BLL implementation and tests.

Possible conflict areas to watch:

- `database/schema.sql` and `database/seed.sql`: Du/Hung must describe changes and wait for Chau review.
- `src/QuanLyKhoBanHang.BLL/Services/*.cs`: Du/Hung may replace stub internals, but should keep public method signatures unless an OpenSpec change documents the contract update.
- `src/QuanLyKhoBanHang.DTO/`: adding properties is safer than renaming/removing existing ones.
- `tests/QuanLyKhoBanHang.Tests/`: tests can be added freely, but avoid deleting existing demo/auth tests.

To keep merge light:

- Du/Hung should pull/rebase after Chau's branch is merged to `main`.
- Du/Hung should not restyle UI screens.
- Any DTO/public service signature change must be called out in PR.
- DAL queries must use parameters for all user input.

## Demo Accounts

- `admin/admin123` -> Admin
- `manager/123456` -> Manager
- `du/123456` -> WarehouseStaff
- `hung/123456` -> SalesStaff

## Final Merge Checklist

- OpenSpec change for the merged phase validates.
- `dotnet build QuanLyKhoBanHang.sln` succeeds.
- `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore` passes.
- WinForms project does not reference DAL.
- WinForms source contains no SQL.
- Four demo accounts still log in.
- Role menus still match Phase 11 expectations.
- Admin screens still open only for Admin.
