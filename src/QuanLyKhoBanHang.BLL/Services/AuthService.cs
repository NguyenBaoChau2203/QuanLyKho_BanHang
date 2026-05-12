using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AuthService
{
    public ServiceResult<UserDto> Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult<UserDto>.Fail("Vui lòng nhập tên đăng nhập và mật khẩu.");
        }

        if (username == "admin" && password == "admin123")
        {
            return ServiceResult<UserDto>.Ok(new UserDto
            {
                Id = 1,
                Username = "admin",
                FullName = "Châu",
                Role = UserRole.Admin
            });
        }

        return ServiceResult<UserDto>.Fail("Thông tin đăng nhập không đúng.");
    }
}
