using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ReportService
{
    public ServiceResult<List<RevenueSummaryDto>> GetRevenue(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<RevenueSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var rows = BuildRevenueRows(fromDate, toDate);
        return ServiceResult<List<RevenueSummaryDto>>.Ok(rows, "Báo cáo doanh thu hiển thị dữ liệu demo nhất quán.");
    }

    public ServiceResult<List<ProductSalesSummaryDto>> GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top = 5)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<ProductSalesSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var rows = BuildTopProducts(fromDate, toDate, top);
        return ServiceResult<List<ProductSalesSummaryDto>>.Ok(rows, "Top sản phẩm hiển thị dữ liệu demo nhất quán.");
    }

    public ServiceResult<List<CustomerPurchaseSummaryDto>> GetTopCustomers(DateTime fromDate, DateTime toDate, int top = 5)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var rows = BuildTopCustomers(fromDate, toDate, top);
        return ServiceResult<List<CustomerPurchaseSummaryDto>>.Ok(rows, "Top khách hàng hiển thị dữ liệu demo nhất quán.");
    }

    private static List<RevenueSummaryDto> BuildRevenueRows(DateTime fromDate, DateTime toDate)
    {
        var today = DateTime.Today;
        if (today < fromDate.Date || today > toDate.Date)
        {
            return [];
        }

        return [new RevenueSummaryDto { Date = today, InvoiceCount = 2, Revenue = 304000, EstimatedProfit = 46000 }];
    }

    private static List<ProductSalesSummaryDto> BuildTopProducts(DateTime fromDate, DateTime toDate, int top)
    {
        if (DateTime.Today < fromDate.Date || DateTime.Today > toDate.Date)
        {
            return [];
        }

        var rows = new List<ProductSalesSummaryDto>
        {
            new() { ProductId = 2, ProductCode = "SP002", ProductName = "Nước ngọt cola lon", QuantitySold = 12, Revenue = 132000 },
            new() { ProductId = 4, ProductCode = "SP004", ProductName = "Nước rửa chén 750ml", QuantitySold = 4, Revenue = 100000 },
            new() { ProductId = 6, ProductCode = "SP006", ProductName = "Khăn giấy 100 tờ", QuantitySold = 2, Revenue = 25000 },
            new() { ProductId = 1, ProductCode = "SP001", ProductName = "Nước suối 500ml", QuantitySold = 10, Revenue = 60000 }
        };

        return rows.Take(Math.Max(1, top)).ToList();
    }

    private static List<CustomerPurchaseSummaryDto> BuildTopCustomers(DateTime fromDate, DateTime toDate, int top)
    {
        if (DateTime.Today < fromDate.Date || DateTime.Today > toDate.Date)
        {
            return [];
        }

        var rows = new List<CustomerPurchaseSummaryDto>
        {
            new() { CustomerId = 2, CustomerName = "Cửa hàng Tạp hóa An Phú", InvoiceCount = 1, TotalAmount = 198000 },
            new() { CustomerId = 1, CustomerName = "Khách lẻ", InvoiceCount = 1, TotalAmount = 106000 },
            new() { CustomerId = 3, CustomerName = "Siêu thị Hòa Bình", InvoiceCount = 0, TotalAmount = 0 }
        };

        return rows.Take(Math.Max(1, top)).ToList();
    }
}
