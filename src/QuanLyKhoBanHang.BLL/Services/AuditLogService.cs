using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Auth;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Admin;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AuditLogService
{
    private readonly AuditLogRepository _auditRepo;

    public AuditLogService()
    {
        var options = new DatabaseOptions();
        _auditRepo = new AuditLogRepository(options);
    }

    public AuditLogService(DatabaseOptions options)
    {
        _auditRepo = new AuditLogRepository(options);
    }

    internal AuditLogService(AuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public ServiceResult<List<AuditLogDto>> GetAuditLogs(DateTime fromDate, DateTime toDate, string? keyword = null)
    {
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1).AddTicks(-1);

        if (from > to)
        {
            return ServiceResult<List<AuditLogDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var normalizedKeyword = keyword?.Trim();
            var logs = _auditRepo.Query(from, to, normalizedKeyword);
            return ServiceResult<List<AuditLogDto>>.Ok(logs, "Đã tải nhật ký hệ thống.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AuditLogDto>>.Fail("Lỗi khi tải nhật ký hệ thống: " + ex.Message);
        }
    }
}
