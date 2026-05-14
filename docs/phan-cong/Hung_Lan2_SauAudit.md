# Phân công lần 2 - Hùng

Ngày lập: 2026-05-15

## Vai trò của Hùng trong vòng 2

Hùng phụ trách sửa và hoàn thiện backend khách hàng, bán hàng, hóa đơn, báo cáo và assistant command liên quan doanh thu/báo cáo.

Trọng tâm vòng này là làm phần bán hàng/báo cáo chạy thật qua BLL/DAL, có transaction đúng và có test nghiệp vụ tối thiểu.

## Branch làm việc đề xuất

```text
fix/sales-report-hung-v2
```

## Bối cảnh hiện tại cần nhớ

- Solution hiện tại chưa build được do lỗi chung ở DAL.
- Phần bán hàng/báo cáo đã có repository thật một phần.
- `CustomerRepository`, `SalesRepository`, `ReportRepository` đang hardcode connection string.
- `SalesService.CreateInvoice(...)` đang trả cứng id `1`, chưa trả invoice id thật.
- UI bán hàng hiện chưa truyền `CreatedByUserId`.
- Dashboard vẫn hardcode số liệu.
- Assistant hiện chưa xử lý đủ command demo.

## P0 - Sửa nền trước

- Chuẩn hóa connection string qua `DatabaseOptions`, không hardcode riêng trong từng repository.
- Đảm bảo repository bán hàng/báo cáo dùng cùng SQL provider với DAL project.
- Không đổi public method nếu không cần.
- Nếu đổi DTO hoặc kiểu trả về thì ghi rõ trong PR.
- Sau khi sửa, chạy:

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

## P1 - Hoàn thiện CustomerService

`CustomerService` cần chạy thật qua DB:

- `GetAllCustomers()`
- `SearchCustomers(keyword)`
- `GetCustomerById(id)`
- `CreateCustomer(customer)`
- `UpdateCustomer(customer)`
- `DeactivateCustomer(id)`

Validation cần có:

- Tên khách hàng là bắt buộc.
- Id hợp lệ khi update/deactivate.
- Check trùng mã khách hàng nếu người dùng nhập mã.

## P1 - Hoàn thiện SalesService

`SalesService.CreateInvoice(invoice)` cần làm đúng:

- Hóa đơn phải có ít nhất một dòng hàng.
- Số lượng bán phải lớn hơn 0.
- Không cho bán vượt tồn kho.
- Tổng tiền hóa đơn bằng tổng thành tiền từng dòng.
- Giảm giá không âm.
- Giảm giá không lớn hơn tổng tiền.
- Nhận và dùng đúng `CreatedByUserId`.
- Lưu `SalesInvoices`.
- Lưu `SalesInvoiceDetails`.
- Giảm `Products.QuantityOnHand`.
- Ghi `StockTransactions`.
- Tất cả nằm trong cùng một SQL transaction.
- Trả về đúng `invoiceId` thật, không trả cứng `1`.

`SalesService.GetInvoices(fromDate, toDate)` cần:

- Lấy danh sách hóa đơn theo ngày.
- Nếu UI cần, nên có thêm tên khách hàng hoặc chuẩn bị DTO phù hợp.

`SalesService.GetInvoiceById(id)` cần:

- Lấy đầy đủ header hóa đơn.
- Lấy đầy đủ danh sách dòng hàng.
- Có đủ dữ liệu để Châu có thể làm in hóa đơn nếu còn thời gian.

## P1 - Hoàn thiện ReportService

`ReportService.GetRevenue(fromDate, toDate)` cần:

- Trả doanh thu theo khoảng ngày.
- Trả số hóa đơn.
- Trả estimated profit nếu có thể tính từ giá vốn.
- Nếu chưa tính profit, ghi rõ trong PR là profit chưa hỗ trợ hoặc đang để `0`.

`ReportService.GetTopSellingProducts(fromDate, toDate, top)` cần:

- Trả sản phẩm bán chạy theo số lượng bán hoặc doanh thu.
- Sắp xếp đúng.
- Lọc đúng theo ngày hóa đơn.

`ReportService.GetTopCustomers(fromDate, toDate, top)` cần:

- Trả khách hàng mua nhiều nhất.
- Sắp xếp đúng theo tổng tiền mua.
- Lọc đúng theo ngày hóa đơn.

## P1 - Hoàn thiện AssistantService command

Assistant cần trả lời được đủ các câu trong checklist demo:

- `doanh thu hôm nay`
- `doanh thu tháng này`
- `top sản phẩm bán chạy`
- `khách hàng mua nhiều nhất`
- `hàng sắp hết`
- `kiểm kê hôm nay`

Hiện tại nhóm command doanh thu/top đã có một phần. Hùng cần bổ sung các command còn thiếu hoặc phối hợp Dũ nếu cần service kho/kiểm kê.

Yêu cầu phản hồi:

- Không crash nếu DB lỗi.
- Có message rõ nếu không có dữ liệu.
- Câu trả lời ngắn gọn, phù hợp demo.

## Test Hùng cần thêm hoặc cập nhật

Hùng cần có test cho các case:

- Tạo khách hàng thiếu tên thì fail.
- Tạo hóa đơn không có dòng hàng thì fail.
- Bán số lượng lớn hơn tồn kho thì fail.
- Bán hàng hợp lệ thì giảm tồn.
- Bán hàng hợp lệ thì ghi giao dịch kho.
- Doanh thu theo ngày trả đúng tổng tiền hóa đơn.
- Top sản phẩm bán chạy sắp xếp đúng.
- Top khách hàng mua nhiều sắp xếp đúng.
- Assistant trả lời đủ command demo.

Nếu test cần database, ghi rõ cách chuẩn bị database trong PR.

## Không thuộc scope chính của Hùng

- Không sửa UI chính nếu Châu chưa yêu cầu.
- Không làm nhập kho/kiểm kê thay Dũ.
- Không làm đăng ký/quên mật khẩu thật.
- Không làm quản lý tài khoản admin thật nếu chưa có OpenSpec riêng.
- Không làm AI online nếu rule-based chưa đủ demo.

## Phần có thể phối hợp với Dũ

- Cập nhật `Products.QuantityOnHand` khi bán hàng.
- Ghi `StockTransactions` khi bán hàng.
- Assistant command `hàng sắp hết` và `kiểm kê hôm nay`.

## Phần mở rộng nếu còn thời gian

Các phần dưới đây là mở rộng, không nên làm trước khi MVP chính ổn:

- In hóa đơn thật.
- Xuất báo cáo Excel thật.
- Báo cáo lợi nhuận chi tiết theo giá vốn.
- Dashboard đọc toàn bộ dữ liệu thật nếu Châu muốn tách dashboard khỏi stub.
- AI online ngoài rule-based.

## Bàn giao PR của Hùng

PR cần ghi rõ:

- Service nào đã hoàn thiện.
- Method nào UI có thể gọi thật.
- Có đổi DTO/schema/public method không.
- Có cần chạy lại `schema.sql` hoặc `seed.sql` không.
- Cách test nhanh luồng bán hàng và báo cáo.
- Phần nào còn chưa làm hoặc cần Châu/Dũ phối hợp.

## Tiêu chí hoàn thành phần Hùng

- Solution build được sau phần sửa của Hùng.
- Khách hàng CRUD chạy qua DB thật.
- Bán hàng giảm tồn, không bán vượt tồn và ghi giao dịch kho.
- Báo cáo đọc dữ liệu thật.
- Assistant trả lời đủ command demo.
- Có test cho nghiệp vụ bán hàng/báo cáo quan trọng.

