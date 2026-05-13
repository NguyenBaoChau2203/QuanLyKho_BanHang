using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Auth;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AuthService
{
    public ServiceResult<UserDto> Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult<UserDto>.Fail("Vui lòng nhập tên đăng nhập và mật khẩu.");
        }

        var user = UserAccountService.AuthenticateDemoUser(username, password);
        if (user is not null)
        {
            return ServiceResult<UserDto>.Ok(user, "Đăng nhập thành công.");
        }

        return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng hoặc tài khoản đã ngừng kích hoạt.");
    }
}
