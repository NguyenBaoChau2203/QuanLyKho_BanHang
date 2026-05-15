using System.Globalization;
using System.Text;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services.Assistant;

public sealed class RuleBasedAssistantProvider
{
    private const string ContextMode = "context";
    private const string ContextStatus = "Dữ liệu an toàn từ BLL.";

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

    public IReadOnlyList<AssistantSafeContext> BuildSafeContexts()
    {
        var contexts = new List<AssistantSafeContext>
        {
            BuildContext("doanh thu hôm nay", "Doanh thu hôm nay"),
            BuildContext("hàng sắp hết", "Sản phẩm sắp hết hàng"),
            BuildContext("top sản phẩm bán chạy", "Top sản phẩm bán chạy"),
            BuildContext("khách hàng mua nhiều nhất", "Khách hàng mua nhiều nhất"),
            BuildContext("kiểm kê hôm nay", "Kiểm kê hôm nay")
        };

        return contexts
            .Where(context => !string.IsNullOrWhiteSpace(context.Intent))
            .ToList();
    }

    public AssistantResponseDto Ask(string question, string mode, string statusMessage, bool isFallback)
    {
        var response = CreateResponse(mode, statusMessage, isFallback);
        if (string.IsNullOrWhiteSpace(question))
        {
            return response;
        }

        var cmd = NormalizeCommand(question);
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        if (ContainsAll(cmd, "doanh", "thu", "hom", "nay"))
        {
            var res = _reportService.GetRevenue(today, today);
            if (res.Success && res.Data is { Count: > 0 })
            {
                var revenue = res.Data.Sum(x => x.Revenue);
                var invoiceCount = res.Data.Sum(x => x.InvoiceCount);
                var profit = res.Data.Sum(x => x.EstimatedProfit);
                response.Answer = $"Doanh thu hôm nay là {revenue:N0} VNĐ với {invoiceCount} hóa đơn. Lợi nhuận ước tính: {profit:N0} VNĐ.";
            }
            else
            {
                response.Answer = "Hôm nay cửa hàng chưa có doanh thu nào.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.RevenueToday;
            return response;
        }

        if (ContainsAll(cmd, "doanh", "thu", "thang", "nay"))
        {
            var res = _reportService.GetRevenue(firstDayOfMonth, today);
            if (res.Success && res.Data is { Count: > 0 })
            {
                var totalRevenue = res.Data.Sum(x => x.Revenue);
                var totalInvoice = res.Data.Sum(x => x.InvoiceCount);
                var totalProfit = res.Data.Sum(x => x.EstimatedProfit);
                response.Answer = $"Doanh thu tháng này đạt {totalRevenue:N0} VNĐ với {totalInvoice} hóa đơn. Lợi nhuận ước tính: {totalProfit:N0} VNĐ.";
            }
            else
            {
                response.Answer = "Tháng này cửa hàng chưa có doanh thu nào.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.RevenueToday;
            return response;
        }

        if (ContainsAny(cmd, "hang sap het", "sap het hang", "ton thap", "can nhap them", "duoi muc ton", "san pham nao sap het"))
        {
            var res = _inventoryService.GetLowStockProducts();
            if (res.Success && res.Data is { Count: > 0 })
            {
                var rows = res.Data
                    .Take(5)
                    .Select(item => $"{item.Code} - {item.Name}: {item.QuantityOnHand}/{item.MinStockLevel} {item.Unit}");
                response.Answer = $"Có {res.Data.Count} sản phẩm sắp hết hàng: {string.Join("; ", rows)}.";
            }
            else
            {
                response.Answer = "Hiện tại không có sản phẩm nào sắp hết hàng theo mức tồn tối thiểu.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.LowStock;
            return response;
        }

        if (ContainsAny(cmd, "top san pham", "ban chay", "san pham ban nhieu"))
        {
            var res = _reportService.GetTopSellingProducts(firstDayOfMonth, today, 5);
            if (res.Success && res.Data is { Count: > 0 })
            {
                var rows = res.Data.Select((item, index) =>
                    $"{index + 1}. {item.ProductName} ({item.QuantitySold:N0} bán ra, {item.Revenue:N0} đ)");
                response.Answer = $"Top sản phẩm bán chạy tháng này: {string.Join("; ", rows)}.";
            }
            else
            {
                response.Answer = "Chưa có dữ liệu sản phẩm bán chạy trong tháng này.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.TopProducts;
            return response;
        }

        if (ContainsAny(cmd, "khach hang", "mua nhieu", "top khach"))
        {
            var res = _reportService.GetTopCustomers(firstDayOfMonth, today, 5);
            if (res.Success && res.Data is { Count: > 0 })
            {
                var rows = res.Data.Select((item, index) =>
                    $"{index + 1}. {item.CustomerName} ({item.InvoiceCount:N0} hóa đơn, {item.TotalAmount:N0} đ)");
                response.Answer = $"Khách hàng mua nhiều nhất tháng này: {string.Join("; ", rows)}.";
            }
            else
            {
                response.Answer = "Chưa có dữ liệu khách hàng mua hàng trong tháng này.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.TopCustomers;
            return response;
        }

        if (ContainsAll(cmd, "kiem", "ke", "hom", "nay"))
        {
            var res = _stocktakeService.GetStocktakes(today, today);
            if (res.Success && res.Data is { Count: > 0 })
            {
                response.Answer = $"Hôm nay đã có {res.Data.Count} phiếu kiểm kê được tạo.";
            }
            else
            {
                response.Answer = "Hôm nay chưa có phiếu kiểm kê nào được tạo.";
            }

            response.Handled = true;
            response.Intent = AssistantIntentCatalog.StocktakeToday;
            return response;
        }

        return response;
    }

    private AssistantSafeContext BuildContext(string question, string title)
    {
        var answer = Ask(question, ContextMode, ContextStatus, isFallback: true);
        return new AssistantSafeContext
        {
            Key = answer.Intent,
            Value = answer.Answer,
            Title = title,
            Intent = answer.Intent,
            Answer = answer.Answer
        };
    }

    private static AssistantResponseDto CreateResponse(string mode, string statusMessage, bool isFallback)
    {
        return new AssistantResponseDto
        {
            Intent = AssistantIntentCatalog.Unknown,
            Handled = false,
            Mode = mode,
            StatusMessage = statusMessage,
            IsFallback = isFallback,
            Answer = "Xin lỗi, tôi chưa hiểu ý bạn. Hiện tại tôi có thể giúp xem doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy, khách hàng mua nhiều nhất hoặc kiểm kê hôm nay."
        };
    }

    private static bool ContainsAny(string source, params string[] terms)
    {
        return terms.Any(source.Contains);
    }

    private static bool ContainsAll(string source, params string[] terms)
    {
        return terms.All(source.Contains);
    }

    private static string NormalizeCommand(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D')
            .ToLowerInvariant()
            .Trim();
    }
}
