using QuanLyKhoBanHang.BLL.Common;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class ServiceResultTests
{
    [TestMethod]
    public void Ok_ShouldReturnSuccessfulResult()
    {
        var result = ServiceResult<int>.Ok(1);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data);
    }

    [TestMethod]
    public void Fail_ShouldReturnFailedResult()
    {
        var result = ServiceResult<int>.Fail("Lỗi");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Lỗi", result.Message);
    }
}
