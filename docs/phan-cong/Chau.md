# Phân công riêng - Châu

## Vai trò

Châu là nhóm trưởng, phụ trách kiến trúc tổng thể, OpenSpec, database tổng thể, toàn bộ WinForms UI, tích hợp service của Dũ/Hùng, dashboard, assistant hybrid, kiểm tra build và demo cuối.

## Branch làm việc

```text
feature/project-ui-chau
```

## Phạm vi chính

- Thiết lập và giữ ổn định kiến trúc project 3 lớp.
- Quản lý OpenSpec, specs, tasks và tiến độ chung.
- Review database schema cuối trước khi demo.
- Làm toàn bộ WinForms UI.
- Tự tích hợp service của Dũ và Hùng vào UI.
- Chuẩn bị báo cáo kỹ thuật và demo cuối.

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

- [ ] UI mở được tất cả menu chính.
- [ ] Không có form nào gọi DAL trực tiếp.
- [ ] Login chạy được với tài khoản seed.
- [ ] Dashboard có layout rõ ràng và sẵn sàng nhận dữ liệu thật.
- [ ] Form nhập kho gọi được `PurchaseService`.
- [ ] Form bán hàng gọi được `SalesService`.
- [ ] Form báo cáo gọi được `ReportService`.
- [ ] Trợ lý quản lý gọi được `AssistantService`.
- [ ] Build solution thành công trước khi merge.
- [ ] Chuẩn bị dữ liệu demo và kịch bản thuyết trình.

## Bàn giao cuối

Châu chịu trách nhiệm merge cuối vào `main`, xử lý conflict, kiểm tra build/test, chạy thử demo và cập nhật checklist demo.
