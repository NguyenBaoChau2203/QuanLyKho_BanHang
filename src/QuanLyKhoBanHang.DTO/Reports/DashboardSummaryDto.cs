namespace QuanLyKhoBanHang.DTO.Reports;

public sealed class DashboardSummaryDto
{
    public decimal TodayRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public int TodayInvoiceCount { get; set; }
    public int LowStockProductCount { get; set; }
    public List<ProductSalesSummaryDto> TopSellingProducts { get; set; } = new();
}
