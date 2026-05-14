# Phân công làm lại lần 2 sau audit merge PR

Ngày lập: 2026-05-15

## Mục tiêu lần 2

Sau khi merge các PR backend/UI vào `main`, nhóm cần làm một vòng sửa có kiểm soát để đưa app từ trạng thái "có nhiều phần demo/stub" sang trạng thái có thể build, chạy demo và kiểm thử nghiệp vụ chính.

Mục tiêu của vòng này:

- Sửa lỗi build trước khi làm thêm chức năng.
- Hoàn thiện đúng phần mỗi người đang phụ trách từ phân công ban đầu.
- Nối UI với service thật ở các luồng đã có backend.
- Tách rõ phần bắt buộc của MVP và phần mở rộng nếu còn thời gian.

## Kết quả audit hiện tại

Trạng thái sau khi pull `main` mới nhất:

- `dotnet build .\QuanLyKhoBanHang.sln` đang fail.
- Lỗi chính: DAL dùng lẫn `System.Data.SqlClient` và `Microsoft.Data.SqlClient`.
- Một số service backend kho yêu cầu `DatabaseOptions` trong constructor, nhưng WinForms đang gọi `new()` không tham số.
- UI nhiều màn hình đã có layout đẹp, nhưng nút lưu/sửa/xóa ở nhiều trang vẫn là stub.
- Login đang là demo account in-memory.
- Đăng ký tài khoản mới và quên mật khẩu chỉ là mô phỏng, chưa làm thật.
- Admin tài khoản, phân quyền và audit log đang là demo/in-memory.
- Dashboard vẫn dùng số liệu hardcode.
- Assistant rule-based chưa xử lý đủ các câu trong checklist demo.

## Nguyên tắc làm vòng 2

- Không làm thêm feature mới trước khi build xanh.
- Không push trực tiếp lên `main`.
- Mỗi người tạo branch mới từ `main` mới nhất.
- Dũ và Hùng không sửa UI chính nếu không được Châu thống nhất.
- WinForms chỉ gọi BLL, không gọi DAL trực tiếp.
- DAL không nối chuỗi SQL từ input người dùng; luôn dùng parameter.
- Nếu đổi DTO, schema hoặc public method thì phải ghi rõ trong PR.
- PR phải có phần "Đã test" với lệnh build/test hoặc lý do chưa test được.

## Thứ tự ưu tiên chung

### P0 - Bắt buộc trước tiên

1. Sửa build toàn solution.
2. Sửa constructor/service wiring để WinForms compile được.
3. Chạy lại:

```powershell
dotnet build .\QuanLyKhoBanHang.sln
dotnet test .\tests\QuanLyKhoBanHang.Tests\QuanLyKhoBanHang.Tests.csproj
```

### P1 - Hoàn thiện MVP thật

1. Master data CRUD thật.
2. Nhập kho tăng tồn và ghi giao dịch kho.
3. Bán hàng giảm tồn, không bán vượt tồn và ghi giao dịch kho.
4. Kiểm kê cập nhật tồn và ghi giao dịch kho.
5. Báo cáo doanh thu/top sản phẩm/top khách hàng đọc dữ liệu thật.
6. Assistant trả lời đủ command demo.

### P2 - Mở rộng nếu còn thời gian

1. Đăng ký tài khoản thật.
2. Quên mật khẩu/đặt lại mật khẩu thật.
3. Auth/Admin/Audit DAL thật.
4. In hóa đơn thật.
5. Xuất Excel thật.
6. AI online ngoài rule-based.

## Phân công Châu

Vai trò vòng 2: tích hợp cuối, UI wiring, scope control, review contract và demo readiness.

Branch đề xuất:

```text
feature/project-integration-chau-v2
```

### Việc bắt buộc

- Review PR của Dũ/Hùng sau khi họ sửa build và backend.
- Kiểm tra WinForms không reference DAL trực tiếp.
- Sau khi Dũ sửa service kho, nối các màn hình sau vào service thật:
  - `FrmProduct`
  - `FrmCategory`
  - `FrmSupplier`
  - `FrmInventory`
  - `FrmPurchaseReceipt`
  - `FrmStocktake`
- Sau khi Hùng sửa service bán hàng/báo cáo, nối các màn hình sau vào service thật:
  - `FrmCustomer`
  - `FrmSalesInvoice`
  - `FrmReport`
  - `FrmAssistant`
- Truyền đúng `CreatedByUserId` từ user đăng nhập vào:
  - phiếu nhập kho,
  - hóa đơn bán hàng,
  - phiếu kiểm kê.
- Rà lại thông báo trên UI để phân biệt rõ:
  - dữ liệu thật,
  - dữ liệu demo,
  - thao tác chưa hỗ trợ.
- Chạy checklist demo cuối trong `docs/06_ChecklistDemo.md`.

### Phần Châu cần quyết định

- Với `Đăng ký tài khoản` và `Quên mật khẩu`:
  - Nếu chỉ demo: ghi rõ là mô phỏng và không đưa vào tiêu chí hoàn thành MVP.
  - Nếu làm thật: tạo OpenSpec change nhỏ cho Auth/Admin real DAL/security trước khi code.

### Không nên ôm trong vòng này nếu chưa cần

- Không tự viết lại toàn bộ backend kho/bán hàng thay Dũ/Hùng.
- Không thêm UI mới lớn ngoài các màn hình đã có.
- Không thêm AI online nếu rule-based chưa đủ demo.

### Bàn giao của Châu

- App build được sau khi merge các PR vòng 2.
- Các menu chính mở không exception.
- Demo account vẫn đăng nhập được:
  - `admin/admin123`
  - `manager/123456`
  - `du/123456`
  - `hung/123456`
- Có ghi chú rõ chức năng nào vẫn demo/stub nếu chưa kịp làm thật.

## Phân công Dũ

Vai trò vòng 2: sửa và hoàn thiện backend kho, master data kho, nhập kho, tồn kho, kiểm kê.

Branch đề xuất:

```text
fix/inventory-du-v2
```

### P0 - Sửa build phần Dũ

- Thống nhất DAL kho dùng `Microsoft.Data.SqlClient` hoặc cập nhật package/reference tương ứng. Khuyến nghị dùng `Microsoft.Data.SqlClient` vì DAL project đã reference package này.
- Sửa các repository kho/master data đang dùng `System.Data.SqlClient`:
  - `CategoryRepository`
  - `ProductRepository`
  - `SupplierRepository`
  - `PurchaseRepository`
  - `StocktakeRepository`
  - `StockTransactionRepository`
- Thêm constructor không tham số cho các service kho để UI hiện tại gọi được, nhưng vẫn giữ constructor nhận `DatabaseOptions` để test/config sau này:
  - `CategoryService`
  - `ProductService`
  - `SupplierService`
  - `InventoryService`
  - `PurchaseService`
  - `StocktakeService`

### P1 - Hoàn thiện nghiệp vụ kho

- `CategoryService`
  - Lấy danh sách loại hàng thật.
  - Tạo/sửa/ngừng kích hoạt thật.
  - Check trùng mã.

- `ProductService`
  - Lấy/tìm sản phẩm thật.
  - Tạo/sửa/ngừng kích hoạt thật.
  - Validate mã, tên, loại hàng, giá nhập, giá bán, tồn kho, tồn tối thiểu.
  - Check trùng mã.

- `SupplierService`
  - Lấy/tìm nhà cung cấp thật.
  - Tạo/sửa/ngừng kích hoạt thật.
  - Check trùng mã.

- `PurchaseService`
  - Tạo phiếu nhập có ít nhất một dòng hàng.
  - Lưu `PurchaseReceipts`.
  - Lưu `PurchaseReceiptDetails`.
  - Tăng `Products.QuantityOnHand`.
  - Ghi `StockTransactions`.
  - Tất cả phải nằm trong cùng một SQL transaction.
  - Không để tình trạng lưu phiếu nhập xong nhưng tăng tồn/giao dịch kho fail ở bước sau.

- `InventoryService`
  - Lấy tồn kho hiện tại từ DB.
  - Lấy hàng tồn thấp theo `QuantityOnHand <= MinStockLevel`.
  - Lấy lịch sử giao dịch kho theo ngày.

- `StocktakeService`
  - Tạo phiếu kiểm kê có ít nhất một dòng.
  - Ghi system quantity, actual quantity, difference.
  - Nếu có chênh lệch thì cập nhật tồn và ghi `StockTransactions`.
  - Tất cả phải nằm trong cùng một SQL transaction.

### Test Dũ cần thêm hoặc cập nhật

- Tạo sản phẩm thiếu mã hoặc tên thì fail.
- Tạo sản phẩm giá/tồn âm thì fail.
- Tạo phiếu nhập rỗng thì fail.
- Tạo phiếu nhập hợp lệ thì tăng tồn và ghi giao dịch kho.
- Sản phẩm dưới mức tồn tối thiểu xuất hiện trong cảnh báo.
- Kiểm kê có chênh lệch thì cập nhật tồn và ghi giao dịch kho.

### Bàn giao của Dũ

PR cần ghi rõ:

- Service nào đã hoàn thiện.
- Method nào UI có thể gọi thật.
- Có đổi DTO/schema/public method không.
- Có cần chạy lại `schema.sql` hoặc `seed.sql` không.
- Lệnh test đã chạy.

## Phân công Hùng

Vai trò vòng 2: sửa và hoàn thiện backend khách hàng, bán hàng, báo cáo và assistant command liên quan doanh thu/báo cáo.

Branch đề xuất:

```text
fix/sales-report-hung-v2
```

### P0 - Sửa nền phần Hùng

- Chuẩn hóa connection string qua `DatabaseOptions`, không hardcode riêng trong từng repository.
- Đảm bảo repository bán hàng/báo cáo dùng cùng provider SQL với DAL project.
- Không đổi public method nếu không thật sự cần.
- Nếu đổi DTO hoặc kiểu trả về thì ghi rõ trong PR.

### P1 - Hoàn thiện nghiệp vụ bán hàng

- `CustomerService`
  - Lấy/tìm khách hàng thật.
  - Tạo/sửa/ngừng kích hoạt thật.
  - Validate tên khách hàng.
  - Check trùng mã khách hàng nếu có nhập mã.

- `SalesService`
  - Tạo hóa đơn có ít nhất một dòng.
  - Validate số lượng bán lớn hơn 0.
  - Không cho bán vượt tồn kho.
  - Validate giảm giá không âm và không lớn hơn tổng tiền.
  - Lưu `SalesInvoices`.
  - Lưu `SalesInvoiceDetails`.
  - Giảm `Products.QuantityOnHand`.
  - Ghi `StockTransactions`.
  - Tất cả nằm trong cùng một SQL transaction.
  - Trả về đúng `invoiceId` thật, không trả cứng `1`.
  - Nhận và dùng đúng `CreatedByUserId` từ UI.

- `SalesService.GetInvoices`
  - Lấy danh sách hóa đơn theo ngày.
  - Nên có thêm thông tin khách hàng nếu UI cần hiển thị.

- `SalesService.GetInvoiceById`
  - Lấy đầy đủ header và lines.
  - Chuẩn bị dữ liệu để Châu có thể làm in hóa đơn nếu còn thời gian.

### P1 - Hoàn thiện báo cáo

- `ReportService.GetRevenue`
  - Doanh thu theo khoảng ngày.
  - Invoice count.
  - Estimated profit nếu có thể tính từ giá vốn; nếu chưa làm thì ghi rõ là chưa hỗ trợ.

- `ReportService.GetTopSellingProducts`
  - Sắp xếp đúng theo số lượng bán hoặc doanh thu.

- `ReportService.GetTopCustomers`
  - Sắp xếp đúng theo tổng tiền mua.

### P1 - Hoàn thiện assistant command

Assistant cần trả lời được các câu trong checklist demo:

- `doanh thu hôm nay`
- `doanh thu tháng này`
- `top sản phẩm bán chạy`
- `khách hàng mua nhiều nhất`
- `hàng sắp hết`
- `kiểm kê hôm nay`

Hiện tại command liên quan doanh thu/top đã có một phần. Hùng cần bổ sung các command còn thiếu hoặc phối hợp Dũ nếu cần dữ liệu kho/kiểm kê.

### Test Hùng cần thêm hoặc cập nhật

- Tạo khách hàng thiếu tên thì fail.
- Tạo hóa đơn không có dòng hàng thì fail.
- Bán số lượng lớn hơn tồn kho thì fail.
- Bán hàng hợp lệ thì giảm tồn và ghi giao dịch kho.
- Doanh thu theo ngày trả đúng tổng tiền hóa đơn.
- Top sản phẩm bán chạy sắp xếp đúng.
- Top khách hàng sắp xếp đúng.
- Assistant trả lời đủ command demo.

### Bàn giao của Hùng

PR cần ghi rõ:

- Service nào đã hoàn thiện.
- Method nào UI có thể gọi thật.
- Có đổi DTO/schema/public method không.
- Có cần chạy lại `schema.sql` hoặc `seed.sql` không.
- Cách test nhanh luồng bán hàng và báo cáo.

## Phần mở rộng so với phân công ban đầu

Các phần dưới đây không phải toàn bộ đều nằm trong phân công ban đầu. Nếu làm thì nên xem là scope mở rộng hoặc phase riêng:

### Mở rộng rõ ràng

- Đăng ký tài khoản thật từ màn hình login.
- Quên mật khẩu/khôi phục mật khẩu thật.
- Gửi email khôi phục mật khẩu.
- Hash password và auth/security thật thay vì demo password trong bộ nhớ.
- Quản lý tài khoản thật bằng database.
- Ma trận phân quyền chỉnh sửa được trong database.
- Audit log ghi thật từ mọi thao tác.
- In hóa đơn thật.
- Xuất Excel thật.
- AI online ngoài rule-based.

### Có trong MVP hoặc checklist nhưng hiện mới demo một phần

- Đăng nhập và phân quyền cơ bản.
- Audit log ở mức điểm nâng cấp.
- Dashboard dữ liệu thật.
- Assistant rule-based.
- Báo cáo doanh thu/top sản phẩm/top khách hàng.

### Không phải mở rộng, mà là bắt buộc sửa do integration

- Sửa build.
- Thống nhất SQL provider.
- Thống nhất `DatabaseOptions`.
- Truyền `CreatedByUserId` từ UI sang service.
- Chuyển các nút lưu/sửa/xóa từ stub sang gọi service thật ở các màn hình MVP.

## Checklist review PR vòng 2

Trước khi Châu merge PR vòng 2, PR phải đạt:

- `dotnet build .\QuanLyKhoBanHang.sln` pass.
- Test liên quan pass hoặc có lý do rõ nếu chưa chạy được.
- Không có WinForms gọi DAL trực tiếp.
- Không có SQL trong WinForms.
- Không có nối chuỗi SQL từ input người dùng.
- Không đổi schema/DTO/public method âm thầm.
- PR description ghi rõ:
  - đã sửa gì,
  - file/service chính,
  - cách test,
  - phần nào còn chưa làm.

## Khuyến nghị thứ tự merge

1. Merge PR sửa build và service constructor trước.
2. Merge backend kho của Dũ.
3. Merge backend bán hàng/báo cáo của Hùng.
4. Châu làm PR tích hợp UI service thật.
5. Chạy demo checklist cuối và chỉ ghi nhận phần mở rộng nếu còn thời gian.
