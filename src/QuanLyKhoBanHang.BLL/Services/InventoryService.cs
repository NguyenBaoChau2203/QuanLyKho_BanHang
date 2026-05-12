using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class InventoryService
{
    public ServiceResult<List<ProductDto>> GetCurrentStock()
    {
        return ServiceResult<List<ProductDto>>.Ok(new List<ProductDto>(), "Chưa có dữ liệu tồn kho.");
    }

    public ServiceResult<List<ProductDto>> GetLowStockProducts()
    {
        return ServiceResult<List<ProductDto>>.Ok(new List<ProductDto>(), "Chưa có dữ liệu cảnh báo tồn thấp.");
    }

    public ServiceResult<List<StockTransactionDto>> GetStockTransactions(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        return ServiceResult<List<StockTransactionDto>>.Ok(new List<StockTransactionDto>(), "Chưa có dữ liệu lịch sử kho.");
    }
}
