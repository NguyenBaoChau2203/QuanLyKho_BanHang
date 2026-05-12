## 1. Demo-ready database review

- [x] 1.1 Review `database/schema.sql` for contract-preserving adjustments only.
- [x] 1.2 Review `database/seed.sql` and expand demo data to cover login, dashboard, categories, suppliers, customers, products, starting stock, and a low-stock example.
- [x] 1.3 Update `database/README.md` with the demo account and seed guidance.

## 2. Validation

- [x] 2.1 Run `openspec validate phase-1-database-demo-readiness`.
- [x] 2.2 Run `dotnet build QuanLyKhoBanHang.sln`.
- [x] 2.3 Run `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.

## 3. Delivery

- [ ] 3.1 Confirm phase 1 stays within database/demo-readiness scope only.
- [ ] 3.2 Commit the phase 1 changes separately.
