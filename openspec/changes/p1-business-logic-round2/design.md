# Design: Hoàn thiện nghiệp vụ P1 (Vòng 2)

## Kiến trúc
Đảm bảo tuân thủ "WinForms -> BLL -> DAL". BLL không chứa SQL Transaction. DAL chứa toàn bộ các thao tác `SqlTransaction`.

## Thay đổi BLL
- `ProductService`: Thêm check `CostPrice >= 0`, `SellingPrice >= 0`, `QuantityOnHand >= 0`.
- `PurchaseService`: Chuẩn bị danh sách Product Quantity Updates và StockTransactions, sau đó gọi xuống DAL (`PurchaseRepository.CreateReceiptWithTransaction`).
- `StocktakeService`: Lấy tồn hệ thống, tính toán lệch, chuẩn bị danh sách Product Quantity Updates và StockTransactions, gọi xuống DAL (`StocktakeRepository.CreateStocktakeWithTransaction`).

## Thay đổi DAL
- Bổ sung `CreateReceiptWithTransaction` vào `PurchaseRepository` nhận danh sách `StockTransactionDto` và thực hiện:
  1. Insert PurchaseReceipts.
  2. Insert PurchaseReceiptDetails.
  3. Update Products.
  4. Insert StockTransactions.
- Bổ sung `CreateStocktakeWithTransaction` vào `StocktakeRepository` nhận danh sách `StockTransactionDto` và thực hiện:
  1. Insert Stocktakes.
  2. Insert StocktakeDetails.
  3. Update Products.
  4. Insert StockTransactions.
