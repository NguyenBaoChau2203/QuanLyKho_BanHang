using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class DashboardService
{
    public ServiceResult<DashboardSummaryDto> GetDashboardSummary(DateTime today)
    {
        var summary = new DashboardSummaryDto
        {
            TodayRevenue = 304000,
            MonthRevenue = 304000,
            TodayInvoiceCount = 2,
            LowStockProductCount = 2,
            TopSellingProducts =
            [
                new ProductSalesSummaryDto { ProductId = 2, ProductCode = "SP002", ProductName = "Nước ngọt cola lon", QuantitySold = 12, Revenue = 132000 },
                new ProductSalesSummaryDto { ProductId = 4, ProductCode = "SP004", ProductName = "Nước rửa chén 750ml", QuantitySold = 4, Revenue = 100000 },
                new ProductSalesSummaryDto { ProductId = 1, ProductCode = "SP001", ProductName = "Nước suối 500ml", QuantitySold = 10, Revenue = 60000 }
            ]
        };

        return ServiceResult<DashboardSummaryDto>.Ok(summary, "Dashboard đang hiển thị dữ liệu demo nhất quán từ seed.");
    }
}
