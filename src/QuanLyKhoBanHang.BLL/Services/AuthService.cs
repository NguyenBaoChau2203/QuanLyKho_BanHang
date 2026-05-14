using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Security;
using QuanLyKhoBanHang.DAL.Auth;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AuthService
{
    private readonly UserRepository _userRepo;
    private readonly AuditLogRepository _auditRepo;

    public AuthService()
    {
        var options = new DatabaseOptions();
        _userRepo = new UserRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    public AuthService(DatabaseOptions options)
    {
        _userRepo = new UserRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    public ServiceResult<UserDto> Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng hoặc tài khoản đã ngừng kích hoạt.");
        }

        var user = _userRepo.GetByUsername(username.Trim());

        if (user is null)
        {
            _auditRepo.Write(null, "Đăng nhập thất bại", "Auth", null,
                $"Tên đăng nhập không tồn tại.");
            return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng hoặc tài khoản đã ngừng kích hoạt.");
        }

        if (!user.IsActive)
        {
            _auditRepo.Write(user.Id, "Đăng nhập thất bại", "Auth", user.Id,
                "Tài khoản đã bị ngừng kích hoạt.");
            return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng hoặc tài khoản đã ngừng kích hoạt.");
        }

        var storedHash = _userRepo.GetPasswordHash(user.Id);
        if (storedHash is null || !PasswordHasher.Verify(password, storedHash))
        {
            _auditRepo.Write(user.Id, "Đăng nhập thất bại", "Auth", user.Id,
                "Mật khẩu không đúng.");
            return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng hoặc tài khoản đã ngừng kích hoạt.");
        }

        _userRepo.UpdateLastLogin(user.Id);
        _auditRepo.Write(user.Id, "Đăng nhập thành công", "Auth", user.Id, null);

        return ServiceResult<UserDto>.Ok(user, "Đăng nhập thành công.");
    }

    public ServiceResult<UserDto> AuthenticateWithPasswordChangeCheck(string username, string password)
    {
        var result = Authenticate(username, password);
        if (!result.Success || result.Data is null)
            return result;

        var storedHash = _userRepo.GetPasswordHash(result.Data.Id);
        if (storedHash is null)
            return result;

        var userRecord = _userRepo.GetByUsername(username.Trim());
        if (userRecord is null)
            return result;

        return ServiceResult<UserDto>.Ok(result.Data, "Đăng nhập thành công.");
    }
}
