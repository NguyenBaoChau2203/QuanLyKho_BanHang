using System.Globalization;
using System.Text;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services.Assistant;

internal sealed class RuleBasedAssistantProvider
{
    private readonly ReportService _reportService;
    private readonly InventoryService _inventoryService;
    private readonly StocktakeService _stocktakeService;

    public RuleBasedAssistantProvider(
        ReportService reportService,
        InventoryService inventoryService,
        StocktakeService stocktakeService)
    {
        _reportService = reportService;
        _inventoryService = inventoryService;
        _stocktakeService = stocktakeService;
    }

    public AssistantResponseDto Ask(string question, string mode, string statusMessage, bool isFallback)
    {
        var intent = ResolveIntent(question);
        return new AssistantResponseDto
        {
            Intent = intent,
            Answer = BuildAnswer(intent),
            Handled = intent != AssistantIntentCatalog.Unknown,
            Mode = mode,
            StatusMessage = statusMessage,
            IsFallback = isFallback
        };
    }

    public IReadOnlyList<AssistantSafeContext> BuildSafeContexts()
    {
        return
        [
            new AssistantSafeContext(AssistantIntentCatalog.RevenueToday, "Doanh thu hôm nay", BuildAnswer(AssistantIntentCatalog.RevenueToday)),
            new AssistantSafeContext(AssistantIntentCatalog.LowStock, "Hàng sắp hết", BuildAnswer(AssistantIntentCatalog.LowStock)),
            new AssistantSafeContext(AssistantIntentCatalog.TopProducts, "Top sản phẩm bán chạy", BuildAnswer(AssistantIntentCatalog.TopProducts)),
            new AssistantSafeContext(AssistantIntentCatalog.TopCustomers, "Khách hàng mua nhiều nhất", BuildAnswer(AssistantIntentCatalog.TopCustomers)),
            new AssistantSafeContext(AssistantIntentCatalog.StocktakeToday, "Kiểm kê hôm nay", BuildAnswer(AssistantIntentCatalog.StocktakeToday))
        ];
    }

    private static string ResolveIntent(string question)
    {
        var normalized = Normalize(question);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return AssistantIntentCatalog.Unknown;
        }

        if (normalized.Contains("doanh thu", StringComparison.Ordinal))
        {
            return AssistantIntentCatalog.RevenueToday;
        }

        if (normalized.Contains("sap het", StringComparison.Ordinal)
            || normalized.Contains("ton thap", StringComparison.Ordinal)
            || normalized.Contains("low stock", StringComparison.Ordinal)
            || normalized.Contains("can nhap", StringComparison.Ordinal))
        {
            return AssistantIntentCatalog.LowStock;
        }

        if ((normalized.Contains("top", StringComparison.Ordinal) || normalized.Contains("ban chay", StringComparison.Ordinal))
            && (normalized.Contains("san pham", StringComparison.Ordinal)
                || normalized.Contains("mat hang", StringComparison.Ordinal)
                || normalized.Contains("hang", StringComparison.Ordinal)))
        {
            return AssistantIntentCatalog.TopProducts;
        }

        if (normalized.Contains("khach", StringComparison.Ordinal)
            && (normalized.Contains("mua", StringComparison.Ordinal)
                || normalized.Contains("nhieu", StringComparison.Ordinal)
                || normalized.Contains("top", StringComparison.Ordinal)))
        {
            return AssistantIntentCatalog.TopCustomers;
        }

        if (normalized.Contains("kiem ke", StringComparison.Ordinal)
            || normalized.Contains("stocktake", StringComparison.Ordinal))
        {
            return AssistantIntentCatalog.StocktakeToday;
        }

        return AssistantIntentCatalog.Unknown;
    }

    private string BuildAnswer(string intent)
    {
        return intent switch
        {
            AssistantIntentCatalog.RevenueToday => BuildRevenueAnswer(),
            AssistantIntentCatalog.LowStock => BuildLowStockAnswer(),
            AssistantIntentCatalog.TopProducts => BuildTopProductsAnswer(),
            AssistantIntentCatalog.TopCustomers => BuildTopCustomersAnswer(),
            AssistantIntentCatalog.StocktakeToday => BuildStocktakeAnswer(),
            _ => "Trợ lý chưa hiểu câu này. Hãy thử: doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy, khách hàng mua nhiều nhất, kiểm kê hôm nay."
        };
    }

    private string BuildRevenueAnswer()
    {
        var today = DateTime.Today;
        var result = _reportService.GetRevenue(today, today);
        if (!result.Success || result.Data is not { Count: > 0 } rows)
        {
            return "Không lấy được dữ liệu doanh thu demo.";
        }

        var total = rows.Sum(x => x.Revenue);
        var invoices = rows.Sum(x => x.InvoiceCount);
        var profit = rows.Sum(x => x.EstimatedProfit);
        return $"Doanh thu hôm nay: {total:N0} đ, {invoices:N0} hóa đơn, lợi nhuận ước tính {profit:N0} đ. Dữ liệu lấy từ ReportService.";
    }

    private string BuildLowStockAnswer()
    {
        var result = _inventoryService.GetLowStockProducts();
        var rows = result.Success ? result.Data ?? [] : [];
        if (rows.Count == 0)
        {
            return "Không có sản phẩm tồn thấp trong dữ liệu demo hiện tại.";
        }

        var details = string.Join("; ", rows.Take(5).Select(x => $"{x.Code} - {x.Name}: {x.QuantityOnHand:N0}/{x.MinStockLevel:N0} {x.Unit}"));
        return $"Có {rows.Count:N0} sản phẩm sắp hết hoặc tồn thấp: {details}.";
    }

    private string BuildTopProductsAnswer()
    {
        var today = DateTime.Today;
        var result = _reportService.GetTopSellingProducts(today.AddDays(-29), today, 5);
        var rows = result.Success ? result.Data ?? [] : [];
        if (rows.Count == 0)
        {
            return "Không lấy được dữ liệu top sản phẩm demo.";
        }

        var details = string.Join("; ", rows.Select((x, index) => $"{index + 1}. {x.ProductName} ({x.QuantitySold:N0} bán ra, {x.Revenue:N0} đ)"));
        return $"Top sản phẩm bán chạy 30 ngày gần nhất: {details}.";
    }

    private string BuildTopCustomersAnswer()
    {
        var today = DateTime.Today;
        var result = _reportService.GetTopCustomers(today.AddDays(-29), today, 5);
        var rows = result.Success ? result.Data ?? [] : [];
        if (rows.Count == 0)
        {
            return "Không lấy được dữ liệu top khách hàng demo.";
        }

        var details = string.Join("; ", rows.Select((x, index) => $"{index + 1}. {x.CustomerName} ({x.InvoiceCount:N0} hóa đơn, {x.TotalAmount:N0} đ)"));
        return $"Khách hàng mua nhiều nhất 30 ngày gần nhất: {details}.";
    }

    private string BuildStocktakeAnswer()
    {
        var today = DateTime.Today;
        var result = _stocktakeService.GetStocktakes(today.AddDays(-7), today);
        if (!result.Success || result.Data is not { Count: > 0 } rows)
        {
            return "Không có dữ liệu kiểm kê demo trong 7 ngày gần nhất.";
        }

        var latest = rows.OrderByDescending(x => x.StocktakeDate).First();
        var lineCount = latest.Lines.Count;
        return $"Phiếu kiểm kê gần nhất là {latest.StocktakeCode} ngày {latest.StocktakeDate:dd/MM/yyyy}, có {lineCount:N0} dòng hàng. Ghi chú: {latest.Note}.";
    }

    private static string Normalize(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
