using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.DTO.Auth;

public sealed class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
