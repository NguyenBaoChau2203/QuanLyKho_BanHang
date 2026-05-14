using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL; // Gọi DAL
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ReportService
{
    private readonly ReportRepository _repo;

    public ReportService()
    {
        _repo = new ReportRepository();
    }

    public ServiceResult<List<RevenueSummaryDto>> GetRevenue(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<RevenueSummaryDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var rows = _repo.GetRevenue(fromDate, toDate);
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
            var rows = _repo.GetTopSellingProducts(fromDate, toDate, top);
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
            var rows = _repo.GetTopCustomers(fromDate, toDate, top);
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Ok(rows, "Lấy danh sách top khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CustomerPurchaseSummaryDto>>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }
}
