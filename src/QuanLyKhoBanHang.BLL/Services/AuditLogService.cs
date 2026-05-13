using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Admin;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AuditLogService
{
    public ServiceResult<List<AuditLogDto>> GetAuditLogs(DateTime fromDate, DateTime toDate, string? keyword = null)
    {
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1).AddTicks(-1);

        if (from > to)
        {
            return ServiceResult<List<AuditLogDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var normalizedKeyword = keyword?.Trim();
        var logs = CreateDemoLogs()
            .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
            .Where(x => string.IsNullOrWhiteSpace(normalizedKeyword) || ContainsKeyword(x, normalizedKeyword))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return ServiceResult<List<AuditLogDto>>.Ok(logs, "Đã tải nhật ký hệ thống demo.");
    }

    private static bool ContainsKeyword(AuditLogDto log, string keyword)
    {
        return log.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || log.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || log.Action.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || log.EntityName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || log.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private static List<AuditLogDto> CreateDemoLogs()
    {
        var today = DateTime.Today;
        return
        [
            new AuditLogDto
            {
                Id = 1,
                CreatedAt = today.AddHours(8).AddMinutes(5),
                Username = "admin",
                FullName = "Châu",
                Action = "Đăng nhập",
                EntityName = "Auth",
                Description = "Quản trị viên đăng nhập để chuẩn bị dữ liệu demo."
            },
            new AuditLogDto
            {
                Id = 2,
                CreatedAt = today.AddHours(8).AddMinutes(16),
                Username = "admin",
                FullName = "Châu",
                Action = "Xem phân quyền",
                EntityName = "RolePermission",
                Description = "Mở ma trận phân quyền để kiểm tra phạm vi truy cập theo vai trò."
            },
            new AuditLogDto
            {
                Id = 3,
                CreatedAt = today.AddHours(9).AddMinutes(20),
                Username = "manager",
                FullName = "Quản lý demo",
                Action = "Xem báo cáo",
                EntityName = "Report",
                Description = "Xem báo cáo doanh thu trong kỳ demo."
            },
            new AuditLogDto
            {
                Id = 4,
                CreatedAt = today.AddDays(-1).AddHours(14).AddMinutes(10),
                Username = "du",
                FullName = "Dũ",
                Action = "Kiểm kê",
                EntityName = "Stocktake",
                Description = "Ghi nhận kiểm kê khu vực trưng bày."
            },
            new AuditLogDto
            {
                Id = 5,
                CreatedAt = today.AddDays(-1).AddHours(16).AddMinutes(35),
                Username = "hung",
                FullName = "Hùng",
                Action = "Bán hàng",
                EntityName = "SalesInvoice",
                Description = "Lập hóa đơn bán hàng demo cho khách lẻ."
            },
            new AuditLogDto
            {
                Id = 6,
                CreatedAt = today.AddDays(-2).AddHours(10).AddMinutes(45),
                Username = "admin",
                FullName = "Châu",
                Action = "Tạo tài khoản",
                EntityName = "Users",
                Description = "Tạo bộ tài khoản demo theo vai trò Admin, Manager, Kho và Bán hàng."
            }
        ];
    }
}
