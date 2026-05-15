# Proposal: Hoàn thiện nghiệp vụ P1 (Vòng 2)

## Description
Dự án cần hoàn thiện các quy trình nghiệp vụ cho Master Data, Purchase (Nhập kho), Stocktake (Kiểm kê) và Inventory (Tồn kho) theo đúng yêu cầu P1.

## Scope
- Bổ sung validation giá trị không âm cho Product (Giá và Tồn kho).
- Viết lại quy trình Nhập kho (`PurchaseService.CreateReceipt`) để gộp các thay đổi vào một SQL Transaction duy nhất, đảm bảo tính toàn vẹn dữ liệu (ACID).
- Viết lại quy trình Kiểm kê (`StocktakeService.CreateStocktake`) để dùng SQL Transaction duy nhất.
- Đảm bảo logic Inventory cảnh báo tồn thấp đúng điều kiện (QuantityOnHand <= MinStockLevel).
