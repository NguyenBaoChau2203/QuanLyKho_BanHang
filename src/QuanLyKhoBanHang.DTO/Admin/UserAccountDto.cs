using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.DTO.Admin;

public sealed class UserAccountDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string DemoPassword { get; set; } = string.Empty;
}
