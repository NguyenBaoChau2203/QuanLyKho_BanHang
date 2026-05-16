# Phân công riêng - Dũ

## Vai trò

Dũ phụ trách backend nghiệp vụ kho: sản phẩm, loại hàng, nhà cung cấp, nhập kho, tồn kho, kiểm kê và cảnh báo tồn thấp.

## Branch làm việc

```text
feature/inventory-du
```

## Phạm vi chính

- Đọc và tuân thủ `docs/07_ContractFoundation.md` trước khi code.
- Viết DTO/DAL/BLL cho nghiệp vụ kho.
- Viết repository ADO.NET cho các bảng thuộc kho.
- Viết validation nghiệp vụ trong BLL.
- Viết test cho nhập kho, tồn kho, kiểm kê.
- Cung cấp service contract rõ ràng để Châu tự tích hợp vào UI.

## Bảng database phụ trách

Các bảng dưới đây là phạm vi nghiệp vụ Dũ cần hiểu và viết DAL/BLL. Châu là người phụ trách chính `schema.sql` và `seed.sql`; nếu Dũ cần đổi bảng/cột/index thì phải báo Châu hoặc ghi rõ trong PR.

- `Categories`
- `Products`
- `Suppliers`
- `PurchaseReceipts`
- `PurchaseReceiptDetails`
- `StockTransactions`
- `Stocktakes`
- `StocktakeDetails`

## Service cần triển khai

- `CategoryService`
  - `GetAllCategories()`
  - `CreateCategory(CategoryDto category)`
  - `UpdateCategory(CategoryDto category)`
  - `DeactivateCategory(int id)`

- `ProductService`
  - `GetAllProducts()`
  - `SearchProducts(string keyword)`
  - `GetProductById(int id)`
  - `CreateProduct(ProductDto product)`
  - `UpdateProduct(ProductDto product)`
  - `DeactivateProduct(int id)`

- `SupplierService`
  - `GetAllSuppliers()`
  - `SearchSuppliers(string keyword)`
  - `CreateSupplier(SupplierDto supplier)`
  - `UpdateSupplier(SupplierDto supplier)`
  - `DeactivateSupplier(int id)`

- `PurchaseService`
  - `CreateReceipt(PurchaseReceiptDto receipt)`
  - `GetReceipts(DateTime fromDate, DateTime toDate)`
  - `GetReceiptById(int id)`

- `InventoryService`
  - `GetCurrentStock()`
  - `GetLowStockProducts()`
  - `GetStockTransactions(DateTime fromDate, DateTime toDate)`

- `StocktakeService`
  - `CreateStocktake(StocktakeDto stocktake)`
  - `GetStocktakes(DateTime fromDate, DateTime toDate)`
  - `GetStocktakeById(int id)`

## Quy tắc nghiệp vụ bắt buộc

- Mã sản phẩm, mã loại hàng, mã nhà cung cấp không được trùng.
- Giá nhập, giá bán, số lượng tồn và mức tồn tối thiểu không được âm.
- Phiếu nhập phải có ít nhất một dòng hàng.
- Khi tạo phiếu nhập:
  - Lưu `PurchaseReceipts`.
  - Lưu `PurchaseReceiptDetails`.
  - Tăng `Products.QuantityOnHand`.
  - Ghi `StockTransactions`.
  - Tất cả nằm trong cùng một SQL transaction.
- Cảnh báo tồn thấp lấy các sản phẩm có `QuantityOnHand <= MinStockLevel`.
- Kiểm kê phải ghi nhận số lượng hệ thống, số lượng thực tế và chênh lệch.
- Nếu kiểm kê làm đổi tồn kho, phải ghi `StockTransactions`.

## Quy định DAL

- Chỉ dùng ADO.NET.
- Không nối chuỗi SQL từ input người dùng.
- Luôn dùng parameter.
- Repository không hiển thị MessageBox.
- Repository không xử lý UI.

## Test cần có

- [x] Tạo sản phẩm thiếu mã hoặc tên thì fail.
- [x] Tạo phiếu nhập rỗng thì fail.
- [x] Tạo phiếu nhập hợp lệ thì tăng tồn.
- [x] Sản phẩm dưới mức tồn tối thiểu xuất hiện trong cảnh báo.
- [x] Kiểm kê có chênh lệch thì cập nhật tồn và ghi giao dịch kho.

## Bàn giao cho Châu

Khi tạo pull request, Dũ cần ghi rõ:

- Service nào đã xong.
- Method nào Châu có thể gọi từ UI.
- DTO nào có thay đổi.
- Có cần chạy lại `schema.sql` hoặc cập nhật database không.
- Cách test nhanh từng service.

## Quy định làm song song

- Dũ không cần chờ UI của Châu hoàn thành mới làm backend.
- Dũ code theo database/DTO/service contract đã chốt ở Phase 0.
- Nếu cần đổi bảng/cột/DTO/public method, phải ghi rõ trong PR để Châu review trước khi merge.
- Nếu chỉ đổi implementation bên trong DAL/BLL và không đổi contract, Châu không cần sửa UI.
