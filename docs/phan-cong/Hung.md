# Phân công riêng - Hùng

## Vai trò

Hùng phụ trách backend nghiệp vụ bán hàng: khách hàng, hóa đơn, chi tiết hóa đơn, báo cáo doanh thu, top sản phẩm, khách hàng mua nhiều và command trợ lý liên quan bán hàng/báo cáo.

## Branch làm việc

```text
feature/sales-report-hung
```

## Phạm vi chính

- Viết DTO/DAL/BLL cho khách hàng và bán hàng.
- Viết repository ADO.NET cho hóa đơn và báo cáo.
- Viết validation nghiệp vụ bán hàng trong BLL.
- Viết rule-based assistant commands liên quan doanh thu và báo cáo.
- Viết test cho bán hàng, hóa đơn và báo cáo.
- Cung cấp service contract rõ ràng để Châu tự tích hợp vào UI.

## Bảng database phụ trách

- `Customers`
- `SalesInvoices`
- `SalesInvoiceDetails`
- `Products` ở phần đọc tồn và cập nhật tồn khi bán
- `StockTransactions` ở phần ghi giao dịch bán hàng

## Service cần triển khai

- `CustomerService`
  - `GetAllCustomers()`
  - `SearchCustomers(string keyword)`
  - `GetCustomerById(int id)`
  - `CreateCustomer(CustomerDto customer)`
  - `UpdateCustomer(CustomerDto customer)`
  - `DeactivateCustomer(int id)`

- `SalesService`
  - `CreateInvoice(SalesInvoiceDto invoice)`
  - `GetInvoices(DateTime fromDate, DateTime toDate)`
  - `GetInvoiceById(int id)`
  - `PrintInvoice(int invoiceId)` hoặc chuẩn bị dữ liệu để Châu in từ UI.

- `ReportService`
  - `GetRevenue(DateTime fromDate, DateTime toDate)`
  - `GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top = 5)`
  - `GetTopCustomers(DateTime fromDate, DateTime toDate, int top = 5)`

- `AssistantService`
  - Bổ sung command: `doanh thu hôm nay`.
  - Bổ sung command: `doanh thu tháng này`.
  - Bổ sung command: `top sản phẩm bán chạy`.
  - Bổ sung command: `khách hàng mua nhiều nhất`.

## Quy tắc nghiệp vụ bắt buộc

- Hóa đơn phải có ít nhất một dòng hàng.
- Số lượng bán phải lớn hơn 0.
- Không cho bán vượt tồn kho.
- Khi tạo hóa đơn:
  - Kiểm tra tồn từng sản phẩm.
  - Lưu `SalesInvoices`.
  - Lưu `SalesInvoiceDetails`.
  - Giảm `Products.QuantityOnHand`.
  - Ghi `StockTransactions`.
  - Tất cả nằm trong cùng một SQL transaction.
- Tổng tiền hóa đơn bằng tổng thành tiền từng dòng.
- Giảm giá không được âm và không được lớn hơn tổng tiền.
- Báo cáo doanh thu lọc theo ngày hóa đơn.

## Quy định DAL

- Chỉ dùng ADO.NET.
- Không nối chuỗi SQL từ input người dùng.
- Luôn dùng parameter.
- Repository không hiển thị MessageBox.
- Repository không xử lý UI.

## Test cần có

- [ ] Tạo khách hàng thiếu tên thì fail.
- [ ] Tạo hóa đơn không có dòng hàng thì fail.
- [ ] Bán số lượng lớn hơn tồn kho thì fail.
- [ ] Bán hàng hợp lệ thì giảm tồn và ghi giao dịch kho.
- [ ] Doanh thu theo ngày trả đúng tổng tiền hóa đơn.
- [ ] Top sản phẩm bán chạy sắp xếp đúng theo số lượng bán.
- [ ] Command trợ lý doanh thu/top sản phẩm trả lời được dữ liệu phù hợp.

## Bàn giao cho Châu

Khi tạo pull request, Hùng cần ghi rõ:

- Service nào đã xong.
- Method nào Châu có thể gọi từ UI.
- DTO nào có thay đổi.
- Có cần chạy lại `schema.sql` hoặc cập nhật database không.
- Cách test nhanh luồng bán hàng và báo cáo.
