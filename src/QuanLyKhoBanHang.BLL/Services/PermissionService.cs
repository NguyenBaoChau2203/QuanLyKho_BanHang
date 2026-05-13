using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Common;

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

    private static readonly Dictionary<UserRole, HashSet<string>> RoleFeatures = new()
    {
        [UserRole.Admin] = FeatureDefinitions.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase),
        [UserRole.Manager] =
        [
            FeatureDashboard,
            FeatureProduct,
            FeatureCategory,
            FeatureSupplier,
            FeatureCustomer,
            FeatureInventory,
            FeatureStocktake,
            FeatureReport,
            FeatureAssistant
        ],
        [UserRole.WarehouseStaff] =
        [
            FeatureProduct,
            FeatureCategory,
            FeatureSupplier,
            FeaturePurchaseReceipt,
            FeatureInventory,
            FeatureStocktake
        ],
        [UserRole.SalesStaff] =
        [
            FeatureProduct,
            FeatureCustomer,
            FeatureInventory,
            FeatureSalesInvoice
        ]
    };

    private static readonly UserRole[] OrderedRoles =
    [
        UserRole.Admin,
        UserRole.Manager,
        UserRole.WarehouseStaff,
        UserRole.SalesStaff
    ];

    public ServiceResult<List<RolePermissionDto>> GetPermissionMatrix()
    {
        var rows = OrderedRoles
            .SelectMany(role => FeatureDefinitions.Select(feature => BuildPermission(role, feature)))
            .ToList();

        return ServiceResult<List<RolePermissionDto>>.Ok(rows, "Đã tải ma trận phân quyền demo.");
    }

    public ServiceResult<List<RolePermissionDto>> GetAccessibleFeatures(UserRole role)
    {
        if (!RoleFeatures.ContainsKey(role))
        {
            return ServiceResult<List<RolePermissionDto>>.Fail("Vai trò không hợp lệ.");
        }

        var rows = FeatureDefinitions
            .Where(feature => IsAllowed(role, feature.Key))
            .Select(feature => BuildPermission(role, feature))
            .ToList();

        return ServiceResult<List<RolePermissionDto>>.Ok(rows, "Đã tải menu theo vai trò.");
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

        return ServiceResult<bool>.Ok(IsAllowed(role, featureKey), "Đã kiểm tra quyền truy cập.");
    }

    public ServiceResult<string> GetDefaultFeature(UserRole role)
    {
        var first = FeatureDefinitions.FirstOrDefault(feature => IsAllowed(role, feature.Key));
        if (first is null)
        {
            return ServiceResult<string>.Fail("Vai trò hiện tại chưa được cấp màn hình mặc định.");
        }

        return ServiceResult<string>.Ok(first.Key, "Đã xác định màn hình mặc định.");
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

    private static RolePermissionDto BuildPermission(UserRole role, FeatureDefinition feature)
    {
        return new RolePermissionDto
        {
            Role = role,
            RoleName = GetRoleDisplayName(role),
            FeatureKey = feature.Key,
            FeatureName = feature.Name,
            GroupName = feature.Group,
            CanAccess = IsAllowed(role, feature.Key),
            Note = feature.Note
        };
    }

    private static bool IsAllowed(UserRole role, string featureKey)
    {
        return RoleFeatures.TryGetValue(role, out var features) && features.Contains(featureKey);
    }

    private sealed record FeatureDefinition(string Key, string Name, string Group, string Note);
}
