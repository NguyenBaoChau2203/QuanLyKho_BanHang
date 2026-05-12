# Phân công riêng - Châu

## Vai trò

Châu là nhóm trưởng, phụ trách kiến trúc tổng thể, OpenSpec, database tổng thể, viết và quản lý SQL scripts, toàn bộ WinForms UI, tích hợp service của Dũ/Hùng, dashboard, assistant hybrid, kiểm tra build và demo cuối.

## Branch làm việc

```text
feature/project-ui-chau
```

## Phạm vi chính

- Thực hiện Phase 0 - Contract Foundation để cả nhóm làm song song không chờ nhau.
- Thiết lập và giữ ổn định kiến trúc project 3 lớp.
- Quản lý OpenSpec, specs, tasks và tiến độ chung.
- Thiết kế database tổng thể, viết `database/schema.sql`, viết `database/seed.sql` và review schema cuối trước khi demo.
- Làm toàn bộ WinForms UI.
- Tự tích hợp service của Dũ và Hùng vào UI.
- Chuẩn bị báo cáo kỹ thuật và demo cuối.

## Phase 0 - Contract Foundation

Châu cần chốt sớm các phần sau và merge lên `main` để Dũ/Hùng kéo về làm backend song song:

- Database schema và seed data.
- DTO chính cho sản phẩm, nhập kho, tồn kho, kiểm kê, khách hàng, bán hàng, báo cáo.
- Public method signature của BLL services.
- Mock/stub behavior cho service khi backend thật chưa hoàn thành.
- File ownership và quy định đổi contract.

Sau Phase 0, Châu làm UI dựa trên contract đã chốt, không cần chờ Dũ/Hùng code xong service thật.

## Database cần phụ trách

- Thiết kế schema tổng thể cho SQL Server LocalDB.
- Viết và cập nhật `database/schema.sql`.
- Viết và cập nhật `database/seed.sql`.
- Quy định tên bảng, khóa chính, khóa ngoại, index và constraint.
- Đảm bảo database hỗ trợ đủ nghiệp vụ nhập kho, bán hàng, kiểm kê, dashboard, báo cáo và assistant.
- Review mọi đề xuất thay đổi schema từ Dũ và Hùng trước khi merge.

## Quy định khi làm database

- Database chính dùng SQL Server LocalDB.
- Script phải chạy được theo thứ tự: `schema.sql` trước, `seed.sql` sau.
- Tên bảng dùng tiếng Anh số nhiều: `Products`, `SalesInvoices`, `StockTransactions`.
- Tên cột dùng PascalCase: `ProductId`, `CreatedAt`, `QuantityOnHand`.
- Bảng nghiệp vụ quan trọng phải có khóa ngoại rõ ràng.
- Số tiền dùng `DECIMAL(18,2)`.
- Ngày giờ dùng `DATETIME2`.
- Dữ liệu danh mục không xóa cứng nếu đã phát sinh giao dịch; dùng `IsActive`.
- Nếu Dũ hoặc Hùng cần thêm cột/bảng, hai bạn phải ghi rõ trong PR hoặc báo Châu trước khi sửa script.

## UI cần phụ trách

- `FrmLogin`: đăng nhập.
- `FrmMain`: layout chính và menu điều hướng.
- `FrmDashboard`: doanh thu, tồn thấp, top sản phẩm.
- `FrmProduct`: quản lý sản phẩm.
- `FrmCategory`: quản lý loại hàng.
- `FrmSupplier`: quản lý nhà cung cấp.
- `FrmCustomer`: quản lý khách hàng.
- `FrmPurchaseReceipt`: nhập kho.
- `FrmInventory`: tồn kho.
- `FrmStocktake`: kiểm kê.
- `FrmSalesInvoice`: bán hàng và hóa đơn.
- `FrmReport`: báo cáo.
- `FrmAssistant`: trợ lý quản lý.

## Quy định khi làm UI

- WinForms chỉ gọi service ở tầng BLL.
- Không gọi DAL trực tiếp từ form.
- Không viết SQL trong form.
- Event click chỉ đọc input, gọi service, hiển thị kết quả.
- Nếu service của Dũ/Hùng chưa xong, dùng mock data tạm nhưng phải để comment rõ chỗ cần nối service thật.
- Giao diện hiển thị tiếng Việt, code class/method/variable dùng tiếng Anh.

## Service cần tích hợp từ Dũ

- `ProductService`
- `CategoryService`
- `SupplierService`
- `PurchaseService`
- `InventoryService`
- `StocktakeService`

## Service cần tích hợp từ Hùng

- `CustomerService`
- `SalesService`
- `ReportService`
- Các command rule-based cho `AssistantService` liên quan doanh thu, top sản phẩm, khách hàng mua nhiều.

## Checklist hoàn thành

- [x] UI mở được tất cả menu chính.
- [x] Phase 0 đã chốt database, DTO và service contract đủ để cả nhóm làm song song.
- [x] `database/schema.sql` chạy được trên SQL Server LocalDB.
- [x] `database/seed.sql` tạo được dữ liệu demo.
- [x] Database có đủ bảng cho nhập kho, bán hàng, kiểm kê, dashboard và báo cáo.
- [x] Không có form nào gọi DAL trực tiếp.
- [x] Login chạy được với tài khoản seed.
- [x] Dashboard có layout rõ ràng và sẵn sàng nhận dữ liệu thật.
- [x] Form nhập kho gọi được `PurchaseService`.
- [x] Form bán hàng gọi được `SalesService`.
- [x] Form báo cáo gọi được `ReportService`.
- [x] Trợ lý quản lý gọi được `AssistantService`.
- [x] Build solution thành công trước khi merge.
- [x] Chuẩn bị dữ liệu demo và kịch bản thuyết trình.

## Bàn giao cuối

Châu chịu trách nhiệm merge cuối vào `main`, xử lý conflict, kiểm tra build/test, chạy thử demo và cập nhật checklist demo.
