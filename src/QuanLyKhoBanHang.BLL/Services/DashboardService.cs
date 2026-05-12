using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class DashboardService
{
    public ServiceResult<DashboardSummaryDto> GetDashboardSummary(DateTime today)
    {
        return ServiceResult<DashboardSummaryDto>.Ok(new DashboardSummaryDto(), "Dashboard đang dùng dữ liệu rỗng chờ tích hợp.");
    }
}
