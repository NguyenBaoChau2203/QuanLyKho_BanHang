using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.DTO.Admin;

public sealed class RolePermissionDto
{
    public UserRole Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string FeatureKey { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public bool CanAccess { get; set; }
    public string Note { get; set; } = string.Empty;
}
