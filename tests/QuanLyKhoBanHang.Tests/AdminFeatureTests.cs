using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class AdminFeatureTests
{
    [TestMethod]
    [DataRow("admin", "admin123", UserRole.Admin)]
    [DataRow("manager", "123456", UserRole.Manager)]
    [DataRow("du", "123456", UserRole.WarehouseStaff)]
    [DataRow("hung", "123456", UserRole.SalesStaff)]
    public void AuthService_AuthenticatesDemoRoleAccounts(string username, string password, UserRole expectedRole)
    {
        var service = new AuthService();

        var result = service.Authenticate(username, password);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(expectedRole, result.Data.Role);
    }

    [TestMethod]
    public void PermissionService_BlocksAdminScreensForNonAdminRoles()
    {
        var service = new PermissionService();

        AssertCanAccess(service, UserRole.Manager, PermissionService.FeatureUserManagement, false);
        AssertCanAccess(service, UserRole.WarehouseStaff, PermissionService.FeatureReport, false);
        AssertCanAccess(service, UserRole.SalesStaff, PermissionService.FeaturePurchaseReceipt, false);
        AssertCanAccess(service, UserRole.Admin, PermissionService.FeatureAuditLog, true);
    }

    [TestMethod]
    public void UserAccountService_ReturnsDemoAccountsWithoutPasswords()
    {
        var service = new UserAccountService();

        var result = service.GetAllAccounts();

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        CollectionAssert.IsSubsetOf(
            new[] { "admin", "manager", "du", "hung" },
            result.Data.Select(x => x.Username).ToArray());
        Assert.IsTrue(result.Data.All(x => string.IsNullOrEmpty(x.DemoPassword)));
    }

    [TestMethod]
    public void AuditLogService_FiltersByKeyword()
    {
        var service = new AuditLogService();

        var result = service.GetAuditLogs(DateTime.Today.AddDays(-7), DateTime.Today, "hung");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreNotEqual(0, result.Data.Count);
        Assert.IsTrue(result.Data.All(x => x.Username.Contains("hung", StringComparison.OrdinalIgnoreCase)
            || x.FullName.Contains("hung", StringComparison.OrdinalIgnoreCase)
            || x.Action.Contains("hung", StringComparison.OrdinalIgnoreCase)
            || x.EntityName.Contains("hung", StringComparison.OrdinalIgnoreCase)
            || x.Description.Contains("hung", StringComparison.OrdinalIgnoreCase)));
    }

    private static void AssertCanAccess(PermissionService service, UserRole role, string featureKey, bool expected)
    {
        var result = service.CanAccess(role, featureKey);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(expected, result.Data);
    }
}
