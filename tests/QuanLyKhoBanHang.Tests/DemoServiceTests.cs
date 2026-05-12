using QuanLyKhoBanHang.BLL.Services;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class DemoServiceTests
{
    [TestMethod]
    public void ReportService_GetRevenue_Today_ReturnsExpectedDemoRevenue()
    {
        var service = new ReportService();

        var result = service.GetRevenue(DateTime.Today, DateTime.Today);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.HasCount(1, result.Data);
        Assert.AreEqual(DateTime.Today, result.Data[0].Date.Date);
        Assert.AreEqual(2, result.Data[0].InvoiceCount);
        Assert.AreEqual(304000, result.Data[0].Revenue);
    }

    [TestMethod]
    public void ReportService_GetRevenue_OutsideDemoRange_ReturnsEmptyList()
    {
        var service = new ReportService();

        var result = service.GetRevenue(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-20));

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsEmpty(result.Data);
    }

    [TestMethod]
    public void InventoryService_GetLowStockProducts_ReturnsExpectedDemoItems()
    {
        var service = new InventoryService();

        var result = service.GetLowStockProducts();

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        CollectionAssert.AreEquivalent(new[] { "SP004", "SP005" }, result.Data.Select(x => x.Code).ToArray());
    }

    [TestMethod]
    public void AssistantService_HandlesAllDemoCommands()
    {
        var service = new AssistantService();

        var commands = new[]
        {
            "doanh thu hôm nay",
            "hàng sắp hết",
            "top sản phẩm bán chạy",
            "khách hàng mua nhiều nhất",
            "kiểm kê hôm nay"
        };

        foreach (var command in commands)
        {
            var result = service.Ask(command);

            Assert.IsTrue(result.Success, command);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data.Handled, command);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data.Answer), command);
        }
    }
}
