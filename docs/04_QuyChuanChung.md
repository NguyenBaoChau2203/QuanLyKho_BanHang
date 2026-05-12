# Quy chuẩn chung

## Kiến trúc

- WinForms chỉ gọi BLL.
- BLL xử lý validation và nghiệp vụ.
- DAL chứa SQL, ADO.NET, mapping và transaction.
- DTO chỉ chứa dữ liệu.
- Tests kiểm tra BLL và nghiệp vụ quan trọng.

## Naming

- Form: `FrmLogin`, `FrmMain`, `FrmProduct`, `FrmSalesInvoice`.
- Service: `ProductService`, `SalesService`, `InventoryService`.
- Repository: `ProductRepository`, `SalesRepository`.
- DTO: `ProductDto`, `SalesInvoiceDto`.
- Method service dùng động từ rõ nghĩa: `GetAllProducts`, `CreateInvoice`, `GetRevenue`.

## Service result

Service public nên trả về `ServiceResult<T>`:

- `Success`: thao tác thành công hay thất bại.
- `Message`: thông báo để UI hiển thị.
- `Data`: dữ liệu trả về.

## Database

- Không nối chuỗi SQL từ input người dùng.
- Luôn dùng parameter.
- Nhập kho, bán hàng, kiểm kê phải dùng transaction.
- Không xóa cứng dữ liệu danh mục nếu đã phát sinh giao dịch; dùng `IsActive = 0`.

## UI

- Giao diện hiển thị tiếng Việt.
- Code class/method/variable dùng tiếng Anh.
- Event click chỉ đọc input, gọi service, hiển thị kết quả.
- Không đặt nghiệp vụ phức tạp trong form.

## Báo cáo

- Mỗi tính năng chính cần có ảnh màn hình.
- Báo cáo phải giải thích rõ mô hình 3 lớp.
- Báo cáo phải nêu OpenSpec được dùng để quản lý yêu cầu và task.
