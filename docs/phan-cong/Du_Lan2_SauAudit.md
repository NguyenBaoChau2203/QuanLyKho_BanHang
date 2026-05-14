# Phân công lần 2 - Dũ

Ngày lập: 2026-05-15

## Vai trò của Dũ trong vòng 2

Dũ phụ trách sửa và hoàn thiện backend kho, master data kho, nhập kho, tồn kho và kiểm kê.

Trọng tâm vòng này là làm cho phần kho build được, chạy thật qua BLL/DAL và có test nghiệp vụ tối thiểu.

## Branch làm việc đề xuất

```text
fix/inventory-du-v2
```

## Bối cảnh hiện tại cần nhớ

- Solution hiện tại chưa build được.
- Lỗi chính nằm ở DAL kho/master data đang dùng `System.Data.SqlClient`, trong khi project DAL đã reference `Microsoft.Data.SqlClient`.
- Một số service kho yêu cầu `DatabaseOptions` trong constructor, làm UI đang gọi `new()` bị lệch.
- UI của Châu đã có màn hình kho/master data, nhưng còn nhiều nơi đang fallback/stub vì backend chưa sẵn sàng.

## P0 - Sửa build trước

Dũ cần ưu tiên các việc này trước khi làm nghiệp vụ mới:

- Thống nhất SQL provider trong DAL kho/master data. Khuyến nghị dùng:

```csharp
using Microsoft.Data.SqlClient;
```

- Sửa các file đang dùng provider sai:
  - `CategoryRepository`
  - `ProductRepository`
  - `SupplierRepository`
  - `PurchaseRepository`
  - `StocktakeRepository`
  - `StockTransactionRepository`
- Thêm constructor không tham số cho service kho để UI compile được, nhưng vẫn giữ constructor nhận `DatabaseOptions`:
  - `CategoryService`
  - `ProductService`
  - `SupplierService`
  - `InventoryService`
  - `PurchaseService`
  - `StocktakeService`
- Sau khi sửa, chạy:

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

## P1 - Hoàn thiện master data kho

### CategoryService

- `GetAllCategories()` đọc DB thật.
- `CreateCategory(category)` lưu DB thật.
- `UpdateCategory(category)` cập nhật DB thật.
- `DeactivateCategory(id)` ngừng kích hoạt thật.
- Check trùng mã loại hàng.
- Validate mã và tên bắt buộc.

### ProductService

- `GetAllProducts()` đọc DB thật.
- `SearchProducts(keyword)` tìm DB thật.
- `GetProductById(id)` đọc DB thật.
- `CreateProduct(product)` lưu DB thật.
- `UpdateProduct(product)` cập nhật DB thật.
- `DeactivateProduct(id)` ngừng kích hoạt thật.
- Check trùng mã sản phẩm.
- Validate:
  - mã bắt buộc,
  - tên bắt buộc,
  - loại hàng hợp lệ,
  - giá nhập không âm,
  - giá bán không âm,
  - tồn kho không âm,
  - tồn tối thiểu không âm.

### SupplierService

- `GetAllSuppliers()` đọc DB thật.
- `SearchSuppliers(keyword)` tìm DB thật.
- `CreateSupplier(supplier)` lưu DB thật.
- `UpdateSupplier(supplier)` cập nhật DB thật.
- `DeactivateSupplier(id)` ngừng kích hoạt thật.
- Check trùng mã nhà cung cấp.
- Validate mã và tên bắt buộc.

## P1 - Hoàn thiện nghiệp vụ nhập kho

`PurchaseService.CreateReceipt(receipt)` phải làm đúng các bước:

- Validate mã phiếu nhập.
- Validate nhà cung cấp.
- Validate người tạo.
- Validate phiếu nhập có ít nhất một dòng hàng.
- Validate từng dòng:
  - sản phẩm hợp lệ,
  - số lượng lớn hơn 0,
  - đơn giá không âm.
- Lưu `PurchaseReceipts`.
- Lưu `PurchaseReceiptDetails`.
- Tăng `Products.QuantityOnHand`.
- Ghi `StockTransactions`.
- Toàn bộ các bước trên phải nằm trong cùng một SQL transaction.

Không được để tình trạng lưu phiếu nhập thành công nhưng tăng tồn hoặc ghi giao dịch kho bị lỗi ở bước sau.

## P1 - Hoàn thiện tồn kho

`InventoryService` cần hỗ trợ thật:

- `GetCurrentStock()`
- `GetLowStockProducts()`
- `GetStockTransactions(fromDate, toDate)`
- Nếu giữ `GetStockTransactionsByProduct(...)` thì cũng cần kiểm tra đúng.

Logic tồn thấp:

```text
QuantityOnHand <= MinStockLevel
```

## P1 - Hoàn thiện kiểm kê

`StocktakeService.CreateStocktake(stocktake)` phải làm đúng:

- Validate mã kiểm kê.
- Validate người tạo.
- Validate có ít nhất một dòng hàng.
- Lấy system quantity hiện tại từ DB.
- Lưu `Stocktakes`.
- Lưu `StocktakeDetails`.
- Nếu actual khác system:
  - cập nhật tồn kho,
  - ghi `StockTransactions`.
- Tất cả nằm trong cùng một SQL transaction.

## Test Dũ cần thêm hoặc cập nhật

Dũ cần có test cho các case:

- Tạo sản phẩm thiếu mã hoặc tên thì fail.
- Tạo sản phẩm giá/tồn âm thì fail.
- Tạo phiếu nhập rỗng thì fail.
- Tạo phiếu nhập hợp lệ thì tăng tồn.
- Tạo phiếu nhập hợp lệ thì ghi giao dịch kho.
- Sản phẩm dưới mức tồn tối thiểu xuất hiện trong cảnh báo.
- Kiểm kê có chênh lệch thì cập nhật tồn.
- Kiểm kê có chênh lệch thì ghi giao dịch kho.

Nếu test cần database, ghi rõ cách chuẩn bị database trong PR.

## Không thuộc scope chính của Dũ

- Không sửa UI chính nếu Châu chưa yêu cầu.
- Không làm báo cáo doanh thu/top khách hàng.
- Không làm bán hàng/hóa đơn.
- Không làm quên mật khẩu/đăng ký.
- Không làm AI online.

## Phần có thể phối hợp với Hùng

- `Products.QuantityOnHand` vì cả nhập kho và bán hàng đều cập nhật.
- `StockTransactions` vì nhập kho, bán hàng và kiểm kê đều ghi giao dịch.
- Assistant command `hàng sắp hết` và `kiểm kê hôm nay` nếu Hùng cần dữ liệu từ kho.

## Bàn giao PR của Dũ

PR cần ghi rõ:

- Service nào đã hoàn thiện.
- Method nào UI có thể gọi thật.
- Có đổi DTO/schema/public method không.
- Có cần chạy lại `schema.sql` hoặc `seed.sql` không.
- Lệnh build/test đã chạy.
- Phần nào còn chưa làm hoặc cần Châu/Hùng phối hợp.

## Tiêu chí hoàn thành phần Dũ

- Solution build được sau phần sửa của Dũ.
- Các service kho chạy qua DAL thật.
- Nhập kho tăng tồn và ghi giao dịch kho.
- Kiểm kê cập nhật tồn và ghi giao dịch kho.
- Có test cho nghiệp vụ kho quan trọng.

