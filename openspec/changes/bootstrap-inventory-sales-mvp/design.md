# Design: Bootstrap Inventory Sales MVP

## Architecture

Ứng dụng dùng mô hình 3 lớp:

- WinForms: giao diện và điều hướng.
- BLL: service, validation và nghiệp vụ.
- DAL: ADO.NET, SQL query, mapping và transaction.
- DTO: model truyền dữ liệu.

Luồng phụ thuộc:

```text
WinForms -> BLL -> DAL -> DTO
WinForms -> DTO
BLL -> DTO
Tests -> BLL/DAL/DTO
```

## Service Contract

BLL public methods trả về `ServiceResult<T>` để UI luôn nhận được trạng thái, thông báo và dữ liệu.

Ví dụ:

- `ProductService.GetAllProducts()`
- `PurchaseService.CreateReceipt(receipt)`
- `SalesService.CreateInvoice(invoice)`
- `ReportService.GetRevenue(fromDate, toDate)`

## Database

SQL Server LocalDB là database chính. Các bảng nghiệp vụ quan trọng:

- `Products`, `Categories`, `Suppliers`, `Customers`
- `PurchaseReceipts`, `PurchaseReceiptDetails`
- `SalesInvoices`, `SalesInvoiceDetails`
- `StockTransactions`, `Stocktakes`, `StocktakeDetails`
- `Users`, `Roles`, `AuditLogs`

## Assistant

Trợ lý quản lý dùng hybrid:

- Rule-based command là mặc định để demo ổn định.
- AI API là phần nâng cao nếu còn thời gian.
- AI không được truy cập database trực tiếp; chỉ gọi BLL/report services.
