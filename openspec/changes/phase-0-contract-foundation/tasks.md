## 1. Contract audit and lock

- [x] 1.1 Review `database/schema.sql` and document the exact Phase 0 stabilization decisions for tables, keys, constraints, and demo-supporting fields.
- [x] 1.2 Review `database/seed.sql` and confirm the seed set includes login accounts, master data, products, and enough stock history for UI/demo flows.
- [x] 1.3 Review DTO files in `src/QuanLyKhoBanHang.DTO/` and freeze the exact public shapes used by WinForms and BLL.
- [x] 1.4 Review public service methods in `src/QuanLyKhoBanHang.BLL/Services/` and freeze the final signatures listed in the design.

## 2. Ownership and parallel-work rules

- [x] 2.1 Record the file ownership boundaries for Châu, Dũ, and Hùng in the Phase 0 deliverables.
- [x] 2.2 Record the contract-change rule: any schema, DTO, or public service signature change requires Châu review before merge.
- [x] 2.3 Record the stub/mock rule: services may return seeded or in-memory data, but the public contract must match the final signatures.

## 3. Phase 0 deliverable review

- [x] 3.1 Verify `proposal.md`, `design.md`, `specs/contract-foundation/spec.md`, and `tasks.md` all reflect Phase 0 only.
- [x] 3.2 Verify the change is decision-complete for parallel work and excludes WinForms screen implementation, full backend implementation, and invoice printing.
- [x] 3.3 Prepare a commit checkpoint listing only the files allowed to change in Phase 0: `database/schema.sql`, `database/seed.sql`, DTO files, BLL public service signatures, docs, and OpenSpec artifacts.
- [x] 3.4 Prepare validation by running `openspec validate phase-0-contract-foundation` before implementation begins.
