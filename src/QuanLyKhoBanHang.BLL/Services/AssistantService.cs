using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AssistantService
{
    private readonly ReportService _reportService = new();
    private readonly InventoryService _inventoryService = new();
    private readonly StocktakeService _stocktakeService = new();

    public ServiceResult<AssistantResponseDto> Ask(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return ServiceResult<AssistantResponseDto>.Fail("Vui lòng nhập câu hỏi hoặc câu lệnh.");
        }

        var normalized = question.Trim().ToLowerInvariant();
        var response = new AssistantResponseDto
        {
            Intent = "unknown",
            Handled = true
        };

        if (normalized.Contains("doanh thu hôm nay") || normalized.Contains("doanh thu hom nay"))
        {
            response.Intent = "revenue-today";
            response.Answer = BuildRevenueAnswer();
        }
        else if (normalized.Contains("doanh thu"))
        {
            response.Intent = "revenue";
            response.Answer = BuildRevenueAnswer();
        }
        else if (normalized.Contains("sắp hết") || normalized.Contains("tồn thấp") || normalized.Contains("low stock"))
        {
            response.Intent = "low-stock";
            response.Answer = BuildLowStockAnswer();
        }
        else if (normalized.Contains("top sản phẩm bán chạy") || normalized.Contains("top san pham ban chay") || normalized.Contains("top") || normalized.Contains("bán chạy") || normalized.Contains("ban chay"))
        {
            response.Intent = "top-products";
            response.Answer = BuildTopProductsAnswer();
        }
        else if ((normalized.Contains("khách hàng mua nhiều nhất") || normalized.Contains("khach hang mua nhieu nhat")) || ((normalized.Contains("khách") || normalized.Contains("khach")) && (normalized.Contains("mua") || normalized.Contains("nhiều") || normalized.Contains("nhieu"))))
        {
            response.Intent = "top-customers";
            response.Answer = BuildTopCustomersAnswer();
        }
        else if (normalized.Contains("kiểm kê hôm nay") || normalized.Contains("kiem ke hom nay") || normalized.Contains("kiểm kê") || normalized.Contains("kiem ke") || normalized.Contains("stocktake"))
        {
            response.Intent = "stocktake-today";
            response.Answer = BuildStocktakeAnswer();
        }
        else
        {
            response.Handled = false;
            response.Answer = "Trợ lý chưa hiểu câu này. Hãy thử: doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy, khách hàng mua nhiều nhất, kiểm kê hôm nay.";
        }

        return ServiceResult<AssistantResponseDto>.Ok(response);
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
        return $"Doanh thu hôm nay: {total:N0} đ · {invoices:N0} hóa đơn · Dữ liệu demo từ seed đã sẵn sàng.";
    }

    private string BuildLowStockAnswer()
    {
        var result = _inventoryService.GetLowStockProducts();
        var count = result.Success ? result.Data?.Count ?? 0 : 0;
        return count > 0
            ? $"Có {count:N0} sản phẩm đang ở trạng thái tồn thấp hoặc sắp hết hàng."
            : "Không có dữ liệu tồn thấp demo.";
    }

    private string BuildTopProductsAnswer()
    {
        var today = DateTime.Today;
        var result = _reportService.GetTopSellingProducts(today.AddDays(-29), today, 5);
        var top = result.Success ? result.Data?.FirstOrDefault() : null;
        return top is null
            ? "Không lấy được dữ liệu top sản phẩm demo."
            : $"Top sản phẩm hiện tại là {top.ProductName} ({top.QuantitySold:N0} bán ra, {top.Revenue:N0} đ).";
    }

    private string BuildTopCustomersAnswer()
    {
        var today = DateTime.Today;
        var result = _reportService.GetTopCustomers(today.AddDays(-29), today, 5);
        var top = result.Success ? result.Data?.FirstOrDefault() : null;
        return top is null
            ? "Không lấy được dữ liệu top khách hàng demo."
            : $"Khách hàng mua nhiều nhất hiện tại là {top.CustomerName} với {top.InvoiceCount:N0} hóa đơn, tổng {top.TotalAmount:N0} đ.";
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
        return $"Phiếu kiểm kê gần nhất là {latest.StocktakeCode} ngày {latest.StocktakeDate:dd/MM/yyyy}, có {lineCount:N0} dòng hàng.";
    }
}
