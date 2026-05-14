# Phân công lần 2 - Châu

Ngày lập: 2026-05-15

## Vai trò của Châu trong vòng 2

Châu phụ trách tích hợp cuối, UI wiring, review contract, kiểm soát scope và chuẩn bị demo.

Mục tiêu chính của Châu không phải viết lại toàn bộ backend thay Dũ/Hùng, mà là đảm bảo các phần backend sau khi sửa được nối đúng vào WinForms và app có thể demo mạch lạc.

## Branch làm việc đề xuất

```text
feature/project-integration-chau-v2
```

## Bối cảnh hiện tại cần nhớ

- Solution hiện tại chưa build được.
- UI nhiều màn hình đã có layout, nhưng một số nút lưu/sửa/xóa vẫn là stub.
- Login đang dùng demo account trong bộ nhớ.
- Đăng ký tài khoản mới và quên mật khẩu chỉ là mô phỏng.
- Admin tài khoản, phân quyền, audit log đang là demo/in-memory.
- Dashboard đang dùng số liệu hardcode.
- Dũ phụ trách kho/master data kho.
- Hùng phụ trách khách hàng/bán hàng/báo cáo/assistant command.

## P0 - Việc Châu cần làm sau khi Dũ/Hùng sửa build

- Pull/rebase từ `main` mới nhất trước khi tích hợp.
- Review PR của Dũ/Hùng, đặc biệt các thay đổi:
  - DTO,
  - public method service,
  - database schema/seed,
  - connection string/config,
  - transaction nghiệp vụ.
- Chạy lại:

```powershell
dotnet build .\QuanLyKhoBanHang.sln
dotnet test .\tests\QuanLyKhoBanHang.Tests\QuanLyKhoBanHang.Tests.csproj
```

- Kiểm tra WinForms không reference DAL trực tiếp.
- Kiểm tra WinForms không có SQL.

## P1 - Nối UI với service thật

Sau khi Dũ hoàn thiện backend kho, Châu nối các màn hình sau vào service thật:

- `FrmProduct`
- `FrmCategory`
- `FrmSupplier`
- `FrmInventory`
- `FrmPurchaseReceipt`
- `FrmStocktake`

Sau khi Hùng hoàn thiện backend bán hàng/báo cáo, Châu nối các màn hình sau vào service thật:

- `FrmCustomer`
- `FrmSalesInvoice`
- `FrmReport`
- `FrmAssistant`

Các điểm cần chú ý khi nối UI:

- Nút lưu/sửa/ngừng kích hoạt ở master data không được chỉ báo "stub" nữa nếu service thật đã sẵn sàng.
- `FrmPurchaseReceipt` phải truyền đúng `CreatedByUserId`.
- `FrmSalesInvoice` phải truyền đúng `CreatedByUserId`.
- `FrmStocktake` phải gọi `CreateStocktake`, không chỉ hiển thị thông báo demo.
- Khi backend lỗi, UI phải hiển thị message rõ ràng thay vì crash.
- Nếu vẫn dùng fallback demo ở màn hình nào, label/message phải nói rõ là dữ liệu demo.

## P1 - Checklist demo Châu cần chạy

- Đăng nhập bằng `admin/admin123`.
- Đăng nhập bằng `manager/123456`, `du/123456`, `hung/123456`.
- Kiểm tra role menu:
  - Manager không thấy Admin-only.
  - WarehouseStaff không thấy bán hàng/báo cáo/Admin-only.
  - SalesStaff không thấy nhập kho/kiểm kê/báo cáo/Admin-only.
- Mở từng menu chính không exception:
  - Dashboard
  - Sản phẩm
  - Loại hàng
  - Nhà cung cấp
  - Khách hàng
  - Nhập kho
  - Tồn kho
  - Kiểm kê
  - Bán hàng
  - Báo cáo
  - Trợ lý
  - Tài khoản
  - Phân quyền
  - Nhật ký hệ thống
- Chạy luồng nhập kho.
- Chạy luồng bán hàng.
- Chạy báo cáo doanh thu.
- Hỏi trợ lý các câu demo:
  - `doanh thu hôm nay`
  - `hàng sắp hết`
  - `top sản phẩm bán chạy`
  - `khách hàng mua nhiều nhất`
  - `kiểm kê hôm nay`

## Phần Châu cần quyết định

### Đăng ký tài khoản

Hiện tại màn hình đăng ký chỉ mô phỏng. Châu cần chọn một trong hai hướng:

- Giữ demo: ghi rõ là ngoài scope MVP thật.
- Làm thật: tạo OpenSpec change riêng cho Auth/Admin real DAL/security.

### Quên mật khẩu

Hiện tại quên mật khẩu chỉ mô phỏng. Châu cần chọn một trong hai hướng:

- Giữ demo: hướng dẫn người dùng liên hệ admin để reset.
- Làm thật: cần có reset password flow, lưu token hoặc quy trình admin reset. Không nên làm vội nếu chưa có OpenSpec.

## Phần mở rộng nếu còn thời gian

Các phần dưới đây là mở rộng so với phân công MVP ban đầu:

- Đăng ký tài khoản thật.
- Quên mật khẩu/đặt lại mật khẩu thật.
- Hash password và auth/security thật.
- Admin quản lý tài khoản bằng database.
- Audit log ghi thật từ mọi thao tác.
- In hóa đơn thật.
- Xuất Excel thật.
- AI online ngoài rule-based.

## Không nên làm trong vòng này nếu chưa cần

- Không viết lại backend kho thay Dũ.
- Không viết lại backend bán hàng/báo cáo thay Hùng.
- Không mở rộng UI lớn khi solution còn chưa build xanh.
- Không thêm feature mới nếu chưa có người chịu trách nhiệm test.

## Tiêu chí bàn giao của Châu

- App build được.
- Test hiện có pass hoặc có ghi chú rõ test nào chưa chạy được.
- UI gọi service thật ở các luồng MVP đã được backend hoàn thiện.
- Demo checklist trong `docs/06_ChecklistDemo.md` chạy được.
- File/tài liệu ghi rõ phần nào vẫn demo/stub.

