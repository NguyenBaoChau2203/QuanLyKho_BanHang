using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Auth;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class PermissionService
{
    public const string FeatureDashboard = "dashboard";
    public const string FeatureProduct = "product";
    public const string FeatureCategory = "category";
    public const string FeatureSupplier = "supplier";
    public const string FeatureCustomer = "customer";
    public const string FeaturePurchaseReceipt = "purchase-receipt";
    public const string FeatureInventory = "inventory";
    public const string FeatureStocktake = "stocktake";
    public const string FeatureSalesInvoice = "sales-invoice";
    public const string FeatureReport = "report";
    public const string FeatureAssistant = "assistant";
    public const string FeatureUserManagement = "user-management";
    public const string FeatureRolePermission = "role-permission";
    public const string FeatureAuditLog = "audit-log";

    private static readonly FeatureDefinition[] FeatureDefinitions =
    [
        new(FeatureDashboard, "Dashboard", "Điều hành", "Tổng quan doanh thu, đơn hàng và cảnh báo tồn kho."),
        new(FeatureProduct, "Sản phẩm", "Danh mục", "Tra cứu và quản lý sản phẩm."),
        new(FeatureCategory, "Loại hàng", "Danh mục", "Quản lý nhóm sản phẩm."),
        new(FeatureSupplier, "Nhà cung cấp", "Danh mục", "Quản lý nhà cung cấp."),
        new(FeatureCustomer, "Khách hàng", "Bán hàng", "Quản lý khách hàng."),
        new(FeaturePurchaseReceipt, "Nhập kho", "Kho", "Lập phiếu nhập hàng."),
        new(FeatureInventory, "Tồn kho", "Kho", "Tra cứu tồn kho và giao dịch kho."),
        new(FeatureStocktake, "Kiểm kê", "Kho", "Theo dõi và lập kiểm kê."),
        new(FeatureSalesInvoice, "Bán hàng", "Bán hàng", "Lập hóa đơn bán hàng."),
        new(FeatureReport, "Báo cáo", "Điều hành", "Xem báo cáo doanh thu và top sản phẩm."),
        new(FeatureAssistant, "Trợ lý AI", "Điều hành", "Hỏi nhanh số liệu qua AssistantService."),
        new(FeatureUserManagement, "Tài khoản", "Quản trị", "Quản lý tài khoản demo."),
        new(FeatureRolePermission, "Phân quyền", "Quản trị", "Xem ma trận quyền theo vai trò."),
        new(FeatureAuditLog, "Nhật ký hệ thống", "Quản trị", "Xem nhật ký thao tác demo.")
    ];

    private static readonly UserRole[] OrderedRoles =
    [
        UserRole.Admin,
        UserRole.Manager,
        UserRole.WarehouseStaff,
        UserRole.SalesStaff
    ];

    private readonly Func<int, HashSet<string>> _getFeatureKeysForRole;

    public PermissionService()
    {
        var options = new DatabaseOptions();
        var permissionRepo = new PermissionRepository(options);
        _getFeatureKeysForRole = permissionRepo.GetFeatureKeysForRole;
    }

    public PermissionService(Func<int, IEnumerable<string>> featureKeyProvider)
    {
        ArgumentNullException.ThrowIfNull(featureKeyProvider);
        _getFeatureKeysForRole = roleId => featureKeyProvider(roleId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public ServiceResult<List<RolePermissionDto>> GetPermissionMatrix()
    {
        try
        {
            var rows = new List<RolePermissionDto>();
            foreach (var role in OrderedRoles)
            {
                var keys = role == UserRole.Admin
                    ? []
                    : _getFeatureKeysForRole((int)role);
                foreach (var feature in FeatureDefinitions)
                {
                    var isAllowed = role == UserRole.Admin || keys.Contains(feature.Key);
                    rows.Add(BuildPermission(role, feature, isAllowed));
                }
            }
            return ServiceResult<List<RolePermissionDto>>.Ok(rows, "Đã tải ma trận phân quyền demo.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<RolePermissionDto>>.Fail("Lỗi CSDL khi tải ma trận phân quyền: " + ex.Message);
        }
    }

    public ServiceResult<List<RolePermissionDto>> GetAccessibleFeatures(UserRole role)
    {
        if (role == UserRole.Admin)
        {
            var adminRows = FeatureDefinitions
                .Select(feature => BuildPermission(role, feature, true))
                .ToList();

            return ServiceResult<List<RolePermissionDto>>.Ok(adminRows, "Đã tải menu theo vai trò.");
        }

        try
        {
            var keys = _getFeatureKeysForRole((int)role);
            var rows = FeatureDefinitions
                .Where(feature => keys.Contains(feature.Key))
                .Select(feature => BuildPermission(role, feature, true))
                .ToList();

            return ServiceResult<List<RolePermissionDto>>.Ok(rows, "Đã tải menu theo vai trò.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<RolePermissionDto>>.Fail("Không thể tải quyền từ CSDL: " + ex.Message);
        }
    }

    public ServiceResult<bool> CanAccess(UserRole role, string featureKey)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
        {
            return ServiceResult<bool>.Fail("Mã màn hình không hợp lệ.");
        }

        var exists = FeatureDefinitions.Any(x => x.Key.Equals(featureKey, StringComparison.OrdinalIgnoreCase));
        if (!exists)
        {
            return ServiceResult<bool>.Fail("Màn hình không tồn tại trong ma trận phân quyền.");
        }

        if (role == UserRole.Admin)
        {
            return ServiceResult<bool>.Ok(true, "Đã kiểm tra quyền truy cập.");
        }

        try
        {
            var keys = _getFeatureKeysForRole((int)role);
            return ServiceResult<bool>.Ok(keys.Contains(featureKey), "Đã kiểm tra quyền truy cập.");
        }
        catch
        {
            return ServiceResult<bool>.Fail("Lỗi CSDL khi kiểm tra quyền.");
        }
    }

    public ServiceResult<string> GetDefaultFeature(UserRole role)
    {
        if (role == UserRole.Admin)
        {
            return ServiceResult<string>.Ok(FeatureDefinitions.First().Key, "Đã xác định màn hình mặc định.");
        }

        try
        {
            var keys = _getFeatureKeysForRole((int)role);
            var first = FeatureDefinitions.FirstOrDefault(feature => keys.Contains(feature.Key));
            if (first is null)
            {
                return ServiceResult<string>.Fail("Vai trò hiện tại chưa được cấp màn hình mặc định.");
            }

            return ServiceResult<string>.Ok(first.Key, "Đã xác định màn hình mặc định.");
        }
        catch
        {
            return ServiceResult<string>.Fail("Lỗi truy xuất CSDL.");
        }
    }

    public static string GetRoleDisplayName(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "Quản trị viên",
            UserRole.Manager => "Quản lý",
            UserRole.WarehouseStaff => "Nhân viên kho",
            UserRole.SalesStaff => "Nhân viên bán hàng",
            _ => "Không xác định"
        };
    }

    private static RolePermissionDto BuildPermission(UserRole role, FeatureDefinition feature, bool canAccess)
    {
        return new RolePermissionDto
        {
            Role = role,
            RoleName = GetRoleDisplayName(role),
            FeatureKey = feature.Key,
            FeatureName = feature.Name,
            GroupName = feature.Group,
            CanAccess = canAccess,
            Note = feature.Note
        };
    }

    private sealed record FeatureDefinition(string Key, string Name, string Group, string Note);
}
