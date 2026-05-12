using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class InventoryService
{
    public ServiceResult<List<ProductDto>> GetCurrentStock()
    {
        return ServiceResult<List<ProductDto>>.Ok(BuildProducts(), "Tồn kho demo từ seed đã sẵn sàng.");
    }

    public ServiceResult<List<ProductDto>> GetLowStockProducts()
    {
        var rows = BuildProducts().Where(x => x.QuantityOnHand <= x.MinStockLevel).ToList();
        return ServiceResult<List<ProductDto>>.Ok(rows, "Danh sách tồn thấp demo từ seed đã sẵn sàng.");
    }

    public ServiceResult<List<StockTransactionDto>> GetStockTransactions(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var rows = BuildTransactions().Where(x => x.CreatedAt.Date >= fromDate.Date && x.CreatedAt.Date <= toDate.Date).ToList();
        return ServiceResult<List<StockTransactionDto>>.Ok(rows, "Lịch sử kho demo từ seed đã sẵn sàng.");
    }

    private static List<ProductDto> BuildProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryId = 1, CategoryName = "Đồ uống", Unit = "Chai", CostPrice = 3500, SellingPrice = 6000, QuantityOnHand = 110, MinStockLevel = 30, IsActive = true },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryId = 1, CategoryName = "Đồ uống", Unit = "Lon", CostPrice = 7000, SellingPrice = 11000, QuantityOnHand = 68, MinStockLevel = 25, IsActive = true },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryId = 2, CategoryName = "Thực phẩm", Unit = "Gói", CostPrice = 3000, SellingPrice = 5000, QuantityOnHand = 195, MinStockLevel = 50, IsActive = true },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryId = 3, CategoryName = "Gia dụng", Unit = "Chai", CostPrice = 18000, SellingPrice = 25000, QuantityOnHand = 32, MinStockLevel = 35, IsActive = true },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryId = 4, CategoryName = "Vệ sinh", Unit = "Tuýp", CostPrice = 12000, SellingPrice = 18000, QuantityOnHand = 30, MinStockLevel = 35, IsActive = true },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryId = 4, CategoryName = "Vệ sinh", Unit = "Gói", CostPrice = 8000, SellingPrice = 12500, QuantityOnHand = 118, MinStockLevel = 15, IsActive = true }
    ];

    private static List<StockTransactionDto> BuildTransactions() =>
    [
        new StockTransactionDto { Id = 1, ProductId = 1, ProductName = "Nước suối 500ml", TransactionType = StockTransactionType.Purchase, QuantityChange = 120, QuantityAfter = 120, ReferenceCode = "PN0001", CreatedAt = DateTime.Today.AddDays(-10).AddHours(9), CreatedByUserId = 1, Note = "Nhập hàng demo ban đầu" },
        new StockTransactionDto { Id = 2, ProductId = 2, ProductName = "Nước ngọt cola lon", TransactionType = StockTransactionType.Purchase, QuantityChange = 80, QuantityAfter = 80, ReferenceCode = "PN0001", CreatedAt = DateTime.Today.AddDays(-10).AddHours(9).AddMinutes(10), CreatedByUserId = 1, Note = "Nhập hàng demo ban đầu" },
        new StockTransactionDto { Id = 3, ProductId = 3, ProductName = "Mì gói bò", TransactionType = StockTransactionType.Purchase, QuantityChange = 200, QuantityAfter = 200, ReferenceCode = "PN0001", CreatedAt = DateTime.Today.AddDays(-10).AddHours(9).AddMinutes(20), CreatedByUserId = 1, Note = "Nhập hàng demo ban đầu" },
        new StockTransactionDto { Id = 4, ProductId = 4, ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.Purchase, QuantityChange = 18, QuantityAfter = 36, ReferenceCode = "PN0002", CreatedAt = DateTime.Today.AddDays(-5).AddHours(9), CreatedByUserId = 1, Note = "Bổ sung tồn kho trưng bày" },
        new StockTransactionDto { Id = 5, ProductId = 5, ProductName = "Kem đánh răng 110g", TransactionType = StockTransactionType.Purchase, QuantityChange = 15, QuantityAfter = 30, ReferenceCode = "PN0002", CreatedAt = DateTime.Today.AddDays(-5).AddHours(9).AddMinutes(10), CreatedByUserId = 1, Note = "Bổ sung tồn kho trưng bày" },
        new StockTransactionDto { Id = 6, ProductId = 6, ProductName = "Khăn giấy 100 tờ", TransactionType = StockTransactionType.Purchase, QuantityChange = 60, QuantityAfter = 120, ReferenceCode = "PN0002", CreatedAt = DateTime.Today.AddDays(-5).AddHours(9).AddMinutes(20), CreatedByUserId = 1, Note = "Bổ sung tồn kho trưng bày" },
        new StockTransactionDto { Id = 7, ProductId = 1, ProductName = "Nước suối 500ml", TransactionType = StockTransactionType.Sale, QuantityChange = -10, QuantityAfter = 110, ReferenceCode = "HD0001", CreatedAt = DateTime.Today.AddDays(-2).AddHours(15), CreatedByUserId = 3, Note = "Bán demo quầy lẻ" },
        new StockTransactionDto { Id = 8, ProductId = 3, ProductName = "Mì gói bò", TransactionType = StockTransactionType.Sale, QuantityChange = -5, QuantityAfter = 195, ReferenceCode = "HD0001", CreatedAt = DateTime.Today.AddDays(-2).AddHours(15).AddMinutes(5), CreatedByUserId = 3, Note = "Bán demo quầy lẻ" },
        new StockTransactionDto { Id = 9, ProductId = 6, ProductName = "Khăn giấy 100 tờ", TransactionType = StockTransactionType.Sale, QuantityChange = -2, QuantityAfter = 118, ReferenceCode = "HD0001", CreatedAt = DateTime.Today.AddDays(-2).AddHours(15).AddMinutes(10), CreatedByUserId = 3, Note = "Bán demo quầy lẻ" },
        new StockTransactionDto { Id = 10, ProductId = 2, ProductName = "Nước ngọt cola lon", TransactionType = StockTransactionType.Sale, QuantityChange = -12, QuantityAfter = 68, ReferenceCode = "HD0002", CreatedAt = DateTime.Today.AddDays(-1).AddHours(10), CreatedByUserId = 3, Note = "Bán demo khách sỉ" },
        new StockTransactionDto { Id = 11, ProductId = 4, ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.Sale, QuantityChange = -4, QuantityAfter = 32, ReferenceCode = "HD0002", CreatedAt = DateTime.Today.AddDays(-1).AddHours(10).AddMinutes(10), CreatedByUserId = 3, Note = "Bán demo khách sỉ" },
        new StockTransactionDto { Id = 12, ProductId = 4, ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.StocktakeAdjustment, QuantityChange = -2, QuantityAfter = 30, ReferenceCode = "KK0001", CreatedAt = DateTime.Today.AddDays(-1).AddHours(18), CreatedByUserId = 4, Note = "Kiểm kê demo quầy trưng bày" }
    ];
}
