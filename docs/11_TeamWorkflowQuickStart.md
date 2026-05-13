# Team Workflow Quick Start

File này là bản đọc nhanh cho Dũ và Hùng trước khi bắt đầu làm tiếp backend.

## Đọc Theo Thứ Tự

1. `README.md`
2. `AGENTS.md`
3. `docs/02_PhanCongCongViec.md`
4. File phân công cá nhân:
   - Dũ: `docs/phan-cong/Du.md`
   - Hùng: `docs/phan-cong/Hung.md`
5. `docs/03_WorkflowLamViec.md`
6. `docs/04_QuyChuanChung.md`
7. `docs/07_ContractFoundation.md`
8. Nếu làm Auth/Admin Phase 12: `docs/phan-cong/Phase12AuthAdmin.md`
9. OpenSpec change liên quan trong `openspec/changes/`

## Branch

- Châu: `feature/project-lead-chau`
- Dũ: `feature/inventory-du`
- Hùng: `feature/sales-report-hung`
- Không push trực tiếp lên `main`.

## Workflow Làm Việc

1. Pull code mới nhất từ `main`.
2. Chuyển sang branch cá nhân.
3. Đọc file phân công và OpenSpec liên quan.
4. Code đúng phần được giao.
5. Không sửa UI nếu không được Châu thống nhất.
6. Không đổi DTO/public service/schema nếu chưa ghi rõ trong PR.
7. Chạy build/test.
8. Commit, push branch, tạo PR vào `main`.
9. Ghi rõ trong PR: đã làm gì, đổi contract gì, đổi database gì, cách test nhanh.

## Database

Project dùng SQL Server Developer qua SSMS.

Server:

```text
BaoChau2203
```

Connection string trong code:

```text
Server=BaoChau2203;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True
```

Script nằm ở:

- `database/schema.sql`
- `database/seed.sql`

Thứ tự chạy trong SSMS:

1. `schema.sql`
2. `seed.sql`

Nếu cần tạo lại database từ đầu:

```sql
USE master;
GO

IF DB_ID(N'QuanLyKhoBanHang') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyKhoBanHang SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyKhoBanHang;
END
GO
```

Sau đó chạy lại `schema.sql` rồi `seed.sql`.

## Phạm Vi Của Dũ

Dũ làm backend kho:

- `CategoryService`
- `ProductService`
- `SupplierService`
- `PurchaseService`
- `InventoryService`
- `StocktakeService`
- DAL/repository liên quan kho
- test nghiệp vụ kho

Phase 12 bổ sung cho Dũ:

- Users/Roles/Permissions DAL
- account backend
- password hashing helper nếu Dũ nhận owner account backend

## Phạm Vi Của Hùng

Hùng làm backend bán hàng/báo cáo:

- `CustomerService`
- `SalesService`
- `ReportService`
- assistant commands liên quan doanh thu/top sản phẩm/khách hàng
- DAL/repository liên quan bán hàng/báo cáo
- test nghiệp vụ bán hàng/báo cáo

Phase 12 bổ sung cho Hùng:

- AuditLogs DAL
- audit filtering
- audit writer
- auth/admin audit tests

## Quy Tắc Tránh Conflict

- Dũ/Hùng không restyle hoặc sửa UI WinForms nếu chưa hỏi Châu.
- Châu review mọi thay đổi `database/schema.sql`, `database/seed.sql`.
- Thêm DTO property thì ưu tiên thêm không phá vỡ code cũ.
- Không rename/xóa DTO property hoặc public service method nếu chưa có OpenSpec/PR mô tả.
- DAL chỉ dùng ADO.NET và parameterized SQL.
- WinForms không gọi DAL, không chứa SQL.

## Validation Trước PR

```powershell
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

Nếu có OpenSpec change:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```
