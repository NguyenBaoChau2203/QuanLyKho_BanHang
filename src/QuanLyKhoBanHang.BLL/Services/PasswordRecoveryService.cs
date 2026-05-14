using System.Security.Cryptography;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Auth;
using QuanLyKhoBanHang.DAL.Data;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class PasswordRecoveryService
{
    private readonly UserRepository _userRepo;
    private readonly PasswordRecoveryRepository _recoveryRepo;
    private readonly AuditLogRepository _auditRepo;

    public PasswordRecoveryService()
    {
        var options = new DatabaseOptions();
        _userRepo = new UserRepository(options);
        _recoveryRepo = new PasswordRecoveryRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    public PasswordRecoveryService(DatabaseOptions options)
    {
        _userRepo = new UserRepository(options);
        _recoveryRepo = new PasswordRecoveryRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    public ServiceResult<bool> SubmitForgotPasswordRequest(string username)
    {
        var genericMessage = "Nếu tài khoản tồn tại và đang hoạt động, yêu cầu khôi phục mật khẩu đã được ghi nhận. Vui lòng liên hệ quản trị viên để được hỗ trợ.";

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _auditRepo.Write(null, "Quên mật khẩu", "Auth", null, "Yêu cầu khôi phục với tên đăng nhập trống.");
                return ServiceResult<bool>.Ok(true, genericMessage);
            }

            var user = _userRepo.GetByUsername(username.Trim());

            if (user is null)
            {
                _auditRepo.Write(null, "Quên mật khẩu", "Auth", null, "Yêu cầu khôi phục cho tên đăng nhập không tồn tại.");
                return ServiceResult<bool>.Ok(true, genericMessage);
            }

            if (!user.IsActive)
            {
                return ServiceResult<bool>.Ok(true, genericMessage);
            }

            var requestCode = GenerateRequestCode();
            _recoveryRepo.Create(user.Id, requestCode);

            _auditRepo.Write(user.Id, "Quên mật khẩu", "Auth", user.Id, "Người dùng gửi yêu cầu khôi phục mật khẩu.");

            return ServiceResult<bool>.Ok(true, genericMessage);
        }
        catch
        {
            // Always return generic success even if DB is unavailable
            return ServiceResult<bool>.Ok(true, genericMessage);
        }
    }

    private static string GenerateRequestCode()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
