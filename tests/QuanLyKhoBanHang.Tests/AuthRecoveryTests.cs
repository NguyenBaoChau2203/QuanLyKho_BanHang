using QuanLyKhoBanHang.BLL.Security;
using QuanLyKhoBanHang.BLL.Services;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class AuthRecoveryTests
{
    [TestMethod]
    public void PasswordHasher_HashAndVerify_ReturnsTrue()
    {
        var password = "Test@123";
        var hash = PasswordHasher.Hash(password);

        Assert.IsTrue(PasswordHasher.Verify(password, hash));
    }

    [TestMethod]
    public void PasswordHasher_WrongPassword_ReturnsFalse()
    {
        var hash = PasswordHasher.Hash("CorrectPassword1");

        Assert.IsFalse(PasswordHasher.Verify("WrongPassword1", hash));
    }

    [TestMethod]
    public void PasswordHasher_NullOrEmptyPassword_HashThrows()
    {
        try
        {
            PasswordHasher.Hash("");
            Assert.Fail("Expected ArgumentException for empty password");
        }
        catch (ArgumentException) { }

        try
        {
            PasswordHasher.Hash(null!);
            Assert.Fail("Expected ArgumentException for null password");
        }
        catch (ArgumentException) { }
    }

    [TestMethod]
    public void PasswordHasher_NullOrEmptyStoredHash_VerifyReturnsFalse()
    {
        Assert.IsFalse(PasswordHasher.Verify("password", ""));
        Assert.IsFalse(PasswordHasher.Verify("password", null!));
    }

    [TestMethod]
    public void PasswordHasher_DifferentPasswords_ProduceDifferentHashes()
    {
        var hash1 = PasswordHasher.Hash("PasswordA1");
        var hash2 = PasswordHasher.Hash("PasswordB2");

        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void PasswordHasher_SamePasswordTwice_ProduceDifferentHashes()
    {
        var hash1 = PasswordHasher.Hash("SamePassword1");
        var hash2 = PasswordHasher.Hash("SamePassword1");

        Assert.AreNotEqual(hash1, hash2);

        Assert.IsTrue(PasswordHasher.Verify("SamePassword1", hash1));
        Assert.IsTrue(PasswordHasher.Verify("SamePassword1", hash2));
    }

    [TestMethod]
    public void PasswordHasher_ValidatePasswordPolicy_RejectsInvalid()
    {
        Assert.IsFalse(PasswordHasher.ValidatePasswordPolicy(""));
        Assert.IsFalse(PasswordHasher.ValidatePasswordPolicy("   "));
        Assert.IsFalse(PasswordHasher.ValidatePasswordPolicy("ab"));
        Assert.IsFalse(PasswordHasher.ValidatePasswordPolicy("abc"));
    }

    [TestMethod]
    public void PasswordHasher_ValidatePasswordPolicy_AcceptsValid()
    {
        Assert.IsTrue(PasswordHasher.ValidatePasswordPolicy("abcd"));
        Assert.IsTrue(PasswordHasher.ValidatePasswordPolicy("1234"));
        Assert.IsTrue(PasswordHasher.ValidatePasswordPolicy("Test@123"));
    }

    [TestMethod]
    public void PasswordHasher_Verify_KnownSeedPassword()
    {
        var storedHash = "v1:100000:QWRtaW5TYWx0VjFGb3JTZQ==:0zD2l4lnoQvE1hsg9fyPoCE85OMwuAlYhNmIHV/rOEo=";

        Assert.IsTrue(PasswordHasher.Verify("admin123", storedHash));
    }

    [TestMethod]
    public void PasswordHasher_Verify_KnownSeedPasswordWrong()
    {
        var storedHash = "v1:100000:QWRtaW5TYWx0VjFGb3JTZQ==:0zD2l4lnoQvE1hsg9fyPoCE85OMwuAlYhNmIHV/rOEo=";

        Assert.IsFalse(PasswordHasher.Verify("wrongpassword", storedHash));
    }

    [TestMethod]
    public void PasswordRecoveryService_SubmitWithEmptyUsername_ReturnsGenericMessage()
    {
        var service = new PasswordRecoveryService();

        var result = service.SubmitForgotPasswordRequest("");

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Message.Contains("Nếu tài khoản tồn tại", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void PasswordRecoveryService_SubmitWithNonExistentUser_ReturnsGenericMessage()
    {
        var service = new PasswordRecoveryService();

        var result = service.SubmitForgotPasswordRequest("nonexistent_user_xyz_999");

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Message.Contains("Nếu tài khoản tồn tại", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void PermissionService_BlocksAdminScreensForNonAdmin()
    {
        var service = new PermissionService(roleId => ((DTO.Common.UserRole)roleId) switch
        {
            DTO.Common.UserRole.Manager =>
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
            DTO.Common.UserRole.WarehouseStaff =>
            [
                PermissionService.FeatureProduct,
                PermissionService.FeatureCategory,
                PermissionService.FeatureSupplier,
                PermissionService.FeaturePurchaseReceipt,
                PermissionService.FeatureInventory,
                PermissionService.FeatureStocktake
            ],
            DTO.Common.UserRole.SalesStaff =>
            [
                PermissionService.FeatureProduct,
                PermissionService.FeatureCustomer,
                PermissionService.FeatureInventory,
                PermissionService.FeatureSalesInvoice
            ],
            _ => []
        });

        AssertCanAccess(service, DTO.Common.UserRole.Manager, PermissionService.FeatureUserManagement, false);
        AssertCanAccess(service, DTO.Common.UserRole.WarehouseStaff, PermissionService.FeatureReport, false);
        AssertCanAccess(service, DTO.Common.UserRole.SalesStaff, PermissionService.FeaturePurchaseReceipt, false);
        AssertCanAccess(service, DTO.Common.UserRole.Admin, PermissionService.FeatureAuditLog, true);
    }

    private static void AssertCanAccess(PermissionService service, DTO.Common.UserRole role, string featureKey, bool expected)
    {
        var result = service.CanAccess(role, featureKey);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(expected, result.Data);
    }
}
