using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL; // Gọi DAL
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ReportService
{
    private readonly Func<DateTime, DateTime, List<RevenueSummaryDto>> _getRevenue;
    private readonly Func<DateTime, DateTime, int, List<ProductSalesSummaryDto>> _getTopSellingProducts;
    private readonly Func<DateTime, DateTime, int, List<CustomerPurchaseSummaryDto>> _getTopCustomers;

    public ReportService()
        : this(new ReportRepository())
    {
    }

    public ReportService(ReportRepository repo)
    {
        ArgumentNullException.ThrowIfNull(repo);
        _getRevenue = repo.GetRevenue;
        _getTopSellingProducts = repo.GetTopSellingProducts;
        _getTopCustomers = repo.GetTopCustomers;
    }

    public ReportService(
        Func<DateTime, DateTime, List<RevenueSummaryDto>> getRevenue,
        Func<DateTime, DateTime, int, List<ProductSalesSummaryDto>> getTopSellingProducts,
        Func<DateTime, DateTime, int, List<CustomerPurchaseSummaryDto>> getTopCustomers)
    {
        _getRevenue = getRevenue ?? throw new ArgumentNullException(nameof(getRevenue));
        _getTopSellingProducts = getTopSellingProducts ?? throw new ArgumentNullException(nameof(getTopSellingProducts));
        _getTopCustomers = getTopCustomers ?? throw new ArgumentNullException(nameof(getTopCustomers));
    }

    public ServiceResult<List<RevenueSummaryDto>> GetRevenue(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<RevenueSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var rows = _getRevenue(fromDate, toDate);
            return ServiceResult<List<RevenueSummaryDto>>.Ok(rows, "Lấy dữ liệu doanh thu thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<RevenueSummaryDto>>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }

    public ServiceResult<List<ProductSalesSummaryDto>> GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top = 5)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<ProductSalesSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var rows = _getTopSellingProducts(fromDate, toDate, top);
            return ServiceResult<List<ProductSalesSummaryDto>>.Ok(rows, "Lấy danh sách top sản phẩm thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ProductSalesSummaryDto>>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }

    public ServiceResult<List<CustomerPurchaseSummaryDto>> GetTopCustomers(DateTime fromDate, DateTime toDate, int top = 5)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }
            
        try
        {
            var rows = _getTopCustomers(fromDate, toDate, top);
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Ok(rows, "Lấy danh sách top khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }
}
