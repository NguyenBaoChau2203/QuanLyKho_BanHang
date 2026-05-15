using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class AdminFeatureTests
{
    [TestMethod]
    public void PermissionService_BlocksAdminScreensForNonAdminRoles()
    {
        var service = CreatePermissionService();

        AssertCanAccess(service, UserRole.Manager, PermissionService.FeatureUserManagement, false);
        AssertCanAccess(service, UserRole.WarehouseStaff, PermissionService.FeatureReport, false);
        AssertCanAccess(service, UserRole.SalesStaff, PermissionService.FeaturePurchaseReceipt, false);
        AssertCanAccess(service, UserRole.Admin, PermissionService.FeatureAuditLog, true);
    }

    [TestMethod]
    public void PermissionService_CanAccess_ReturnsCorrectResultForEachRole()
    {
        var service = CreatePermissionService();

        var result = service.CanAccess(UserRole.Admin, PermissionService.FeatureDashboard);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data);

        result = service.CanAccess(UserRole.SalesStaff, PermissionService.FeatureDashboard);
        Assert.IsFalse(result.Data);

        result = service.CanAccess(UserRole.Admin, "nonexistent-key");
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void PermissionService_GetAccessibleFeatures_ReturnsCorrectCount()
    {
        var service = CreatePermissionService();

        var adminFeatures = service.GetAccessibleFeatures(UserRole.Admin);
        Assert.IsTrue(adminFeatures.Success);
        Assert.IsGreaterThanOrEqualTo(13, adminFeatures.Data!.Count);

        var warehouseFeatures = service.GetAccessibleFeatures(UserRole.WarehouseStaff);
        Assert.IsTrue(warehouseFeatures.Success);
        Assert.IsTrue(warehouseFeatures.Data!.All(f => f.CanAccess));

        var salesFeatures = service.GetAccessibleFeatures(UserRole.SalesStaff);
        Assert.IsTrue(salesFeatures.Success);
        Assert.IsNotEmpty(salesFeatures.Data!);
        Assert.IsTrue(salesFeatures.Data!.Any(f => f.FeatureKey == PermissionService.FeatureSalesInvoice));
        Assert.IsTrue(salesFeatures.Data!.Any(f => f.FeatureKey == PermissionService.FeatureCustomer));
        Assert.IsFalse(salesFeatures.Data!.Any(f => f.FeatureKey == PermissionService.FeatureReport));
        Assert.IsFalse(salesFeatures.Data!.Any(f => f.FeatureKey == PermissionService.FeaturePurchaseReceipt));
    }

    private static void AssertCanAccess(PermissionService service, UserRole role, string featureKey, bool expected)
    {
        var result = service.CanAccess(role, featureKey);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(expected, result.Data);
    }

    private static PermissionService CreatePermissionService()
    {
        return new PermissionService(roleId => ((UserRole)roleId) switch
        {
            UserRole.Manager =>
            [
                PermissionService.FeatureDashboard,
                PermissionService.FeatureProduct,
                PermissionService.FeatureCategory,
                PermissionService.FeatureSupplier,
                PermissionService.FeatureCustomer,
                PermissionService.FeatureInventory,
                PermissionService.FeatureStocktake,
                PermissionService.FeatureReport,
                PermissionService.FeatureAssistant
            ],
            UserRole.WarehouseStaff =>
            [
                PermissionService.FeatureProduct,
                PermissionService.FeatureCategory,
                PermissionService.FeatureSupplier,
                PermissionService.FeaturePurchaseReceipt,
                PermissionService.FeatureInventory,
                PermissionService.FeatureStocktake
            ],
            UserRole.SalesStaff =>
            [
                PermissionService.FeatureProduct,
                PermissionService.FeatureCustomer,
                PermissionService.FeatureInventory,
                PermissionService.FeatureSalesInvoice
            ],
            _ => []
        });
    }
}
