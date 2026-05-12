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

        return ServiceResult<List<RevenueSummaryDto>>.Ok(new List<RevenueSummaryDto>(), "Chưa có dữ liệu doanh thu.");
    }

    public ServiceResult<List<ProductSalesSummaryDto>> GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top = 5)
    {
        return ServiceResult<List<ProductSalesSummaryDto>>.Ok(new List<ProductSalesSummaryDto>(), "Chưa có dữ liệu top sản phẩm.");
    }

    public ServiceResult<List<CustomerPurchaseSummaryDto>> GetTopCustomers(DateTime fromDate, DateTime toDate, int top = 5)
    {
        return ServiceResult<List<CustomerPurchaseSummaryDto>>.Ok(new List<CustomerPurchaseSummaryDto>(), "Chưa có dữ liệu khách hàng mua nhiều.");
    }
}
