## 1. Goal

Prepare the shared database scripts for Châu's UI demo and the later Dũ/Hùng backend work without changing the contract surface.

## 2. Scope

This change is limited to the database demo-readiness layer:

- review `database/schema.sql`
- review and improve `database/seed.sql`
- update `database/README.md` if needed

It does not introduce DAL repositories, real database access code, or new UI screens.

## 3. Decisions

### 3.1 Schema stability

- Preserve the existing table and key structure.
- Only add safe, contract-preserving checks or indexes if clearly beneficial.
- Do not rename or remove public business columns used by the MVP contract.

### 3.2 Seed completeness

Seed data must cover:

- login accounts for `admin`, `du`, and `hung`
- enough categories, suppliers, customers, and products for demo flows
- starting stock plus at least one low-stock example
- dashboard-friendly data such as recent receipts, invoices, and stock movements

### 3.3 Documentation

`database/README.md` should describe:

- connection string for LocalDB
- order of running scripts
- demo accounts
- the fact that seed passwords are still text for demo purposes only

## 4. Validation

- `openspec validate phase-1-database-demo-readiness`
- `dotnet build QuanLyKhoBanHang.sln`
- `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`

## 5. Risks

- Accidentally altering schema contract fields.
  - Mitigation: keep the change additive and contract-preserving.
- Seed data not matching dashboard expectations.
  - Mitigation: include receipts, invoices, and stock transactions that produce visible demo data.
